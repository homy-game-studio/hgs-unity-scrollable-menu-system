using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using HGS.SharedUtils;

namespace HGS.ScrollableMenuSystem
{
    public class ScrollableMenu : MonoBehaviour
    {
        [System.Serializable]
        public class ScrollableMenuSettings
        {
            [Header("Swipe")]
            [Tooltip("Tempo limite para deslizar rápido em segundos")]
            public float fastSwipeThresholdTime = 0.3f;
            [Tooltip("Tempo limite para deslizar rapidamente em pixels (sem escala)")]
            public int fastSwipeThresholdDistance = 100;
            [Tooltip("Com que rapidez a página se moverá para a posição de destino")]
            public float decelerationRateTarget = 10f;

            [Header("Position")]
            [Tooltip("Define índice da página inicial começando em 0")]
            public int startingPage = 0;

            [Header("UI")]
            [Tooltip("Botão para ir para a página anterior (opcional)")]
            public Button prevButton = null;
            [Tooltip("Botão para ir para a próxima página (opcional)")]
            public Button nextButton = null;

            [Header("Sprites")]
            [Tooltip("Sprite para página não selecionada (opcional)")]
            public Sprite unselectedPage = null;
            [Tooltip("Sprite para a página selecionada (opcional)")]
            public Sprite selectedPage = null;

            [Header("Container")]
            [Tooltip("Container com botões/Imagens de navegação para as paginas (optional)")]
            public Transform navegationButtonContainer = null;
            public OutterScrollRect scrollRect = null;
        }

        [SerializeField] ScrollableMenuSettings scrollableMenuSettings = null;

        //Se o lerping está em andamento
        bool _isLerpInProgress = false;
        //No arrastar, quando o arrastar começou e onde começou
        bool _isDragging = false;
        //Para mostrar ícones de páginas pequenas
        bool _isShowingPageSelection = false;

        //Número de páginas no container        
        int _pageCount = 0;
        //Indíce da página atual
        int _currentPage = 0;
        //Deslizamentos rápidos devem ser rápidos e curtos. Se for muito longo, então não é um deslizar rápido
        int _fastSwipeThresholdMaxLimit = 0;
        int _previousPageSelectionIndex = 0;

        float _timeStamp = 0f;

        //Posição lerp alvo
        Vector2 _lerpToPosition = Vector2.zero;
        Vector2 _startPosition = Vector2.zero;
        //Posição de destino de cada página
        List<Vector2> _pagePositions = new List<Vector2>();
        //Lista com componentes de imagem - uma imagem para cada página
        List<Image> _pageSelectionImages = null;
        RectTransform _content = null;

        private void Awake()
        {
            _content = scrollableMenuSettings.scrollRect.content;
            if (scrollableMenuSettings.unselectedPage == null) Debugger.Instance.Error("ScrollableMenuManager", "A campo unselectedPage não pode ser nulo");
            if (scrollableMenuSettings.selectedPage == null) Debugger.Instance.Error("ScrollableMenuManager", "A campo selectedPage não pode ser nulo");
            scrollableMenuSettings.scrollRect.onBeginDrag += HandleOnBeginDrag;
            scrollableMenuSettings.scrollRect.onDrag += HandleOnDrag;
            scrollableMenuSettings.scrollRect.onEndDrag += HandleOnEndDrag;
        }

        private void Start()
        {
            _pageCount = _content.childCount;
            _isLerpInProgress = false;

            StartCoroutine(Setup());

            // prev and next buttons
            if (scrollableMenuSettings.nextButton) scrollableMenuSettings.nextButton.onClick.AddListener(() => { NextScreen(); });
            if (scrollableMenuSettings.prevButton) scrollableMenuSettings.prevButton.onClick.AddListener(() => { PreviousScreen(); });

            int buttonCount = scrollableMenuSettings.navegationButtonContainer.childCount;

            // Cacheia todas os componentes de Button dentro da lista
            for (int i = 0; i < buttonCount; i++)
            {
                Button button = scrollableMenuSettings.navegationButtonContainer.GetChild(i).GetComponent<Button>();

                if (button == null)
                {
                    Debugger.Instance.Warn("ScrollableMenuManager", "O botão de seleção de página na posição " + i + " está sem o componente de Button");
                }

                int index = i;
                button.onClick.AddListener(() => LerpToPage(index));
            }
        }

        private void Update()
        {
            // Se movendo para a posição de destino?
            if (_isLerpInProgress)
            {
                //Evitar ultrapassagem de velocidade com valores maiores que 1
                float decelerate = Mathf.Min(scrollableMenuSettings.decelerationRateTarget * Time.deltaTime, 1f);
                _content.anchoredPosition = Vector2.Lerp(_content.anchoredPosition, _lerpToPosition, decelerate);

                //Hora de parar de lerping?
                if (Vector2.SqrMagnitude(_content.anchoredPosition - _lerpToPosition) < 0.25f)
                {
                    //Ajuste a posição alvo e pare o lerping
                    _content.anchoredPosition = _lerpToPosition;
                    _isLerpInProgress = false;

                    //Limpar também qualquer movimento scrollrect que possa interferir com nosso lerping
                    scrollableMenuSettings.scrollRect.velocity = Vector2.zero;
                }

                //Muda o ícone de seleção exatamente para a página correta
                if (_isShowingPageSelection)
                {
                    SetPageSelection(GetNearestPage());
                }
            }
        }

        IEnumerator Setup()
        {
            yield return new WaitForEndOfFrame();
            // Setando a página 0 como inicial e ajustando as demais páginas
            SetPagePositions();
            SetPage(scrollableMenuSettings.startingPage);
            SetupPageSelectionImages();
            SetPageSelection(scrollableMenuSettings.startingPage);
        }

        private void SetPagePositions()
        {
            Rect contentRect = _content.rect;
            int width = (int)contentRect.width;
            int height = (int)contentRect.height;

            if (scrollableMenuSettings.scrollRect.horizontal)
            {
                // Comprimento limite para o deslize rápido - além desse comprimento, não é mais possível deslizar rapidamente
                _fastSwipeThresholdMaxLimit = width;
            }
            else
            {
                _fastSwipeThresholdMaxLimit = height;
            }

            ///Reposicionando paginas ----------------------------------------------

            //Limpa a lista de posições das páginas
            _pagePositions.Clear();

            //Varrer todos os filhos do container e definir suas posições
            for (int i = 0; i < _pageCount; i++)
            {
                RectTransform child = _content.GetChild(i) as RectTransform;
                float childWidth = child.rect.width;
                float childHeight = child.rect.height;
                Vector2 pagePosition = scrollableMenuSettings.scrollRect.horizontal
                    ? new Vector2(childWidth * i, 0f)
                    : new Vector2(0f, childHeight * i);

                _pagePositions.Add(-pagePosition);
            }
        }

        private void SetPage(int pageIndex)
        {
            pageIndex = Mathf.Clamp(pageIndex, 0, _pageCount - 1);
            _content.anchoredPosition = _pagePositions[pageIndex];
            _currentPage = pageIndex;
        }

        private void LerpToPage(int pageIndex)
        {
            pageIndex = Mathf.Clamp(pageIndex, 0, _pageCount - 1);
            _lerpToPosition = _pagePositions[pageIndex];
            _isLerpInProgress = true;
            _currentPage = pageIndex;
        }

        private void SetupPageSelectionImages()
        {
            //Seleção de página - somente se sprites definidos para ícones de seleção
            _isShowingPageSelection = scrollableMenuSettings.unselectedPage != null && scrollableMenuSettings.selectedPage != null;

            if (_isShowingPageSelection)
            {
                bool isNavegationButtonWrongConfiguration =
                    scrollableMenuSettings.navegationButtonContainer == null ||
                    scrollableMenuSettings.navegationButtonContainer.childCount !=
                    _pageCount;

                //Importante: O container PageSelectionContainer com as/os imagens/botões de navegação deve ser definido e deve ter exatamente a mesma quantidade de itens que o container das páginas
                if (isNavegationButtonWrongConfiguration)
                {
                    Debugger.Instance.Error("ScrollableMenuManager", "Contagem diferente de páginas e ícones de seleção - não mostrará a seleção de página");
                    _isShowingPageSelection = false;
                }
                else
                {
                    _previousPageSelectionIndex = -1;
                    _pageSelectionImages = new List<Image>();

                    int buttonCount = scrollableMenuSettings.navegationButtonContainer.childCount;
                    // Cacheia todas os componentes de imagem dentro da lista
                    for (int i = 0; i < buttonCount; i++)
                    {
                        Image image = scrollableMenuSettings.navegationButtonContainer.GetChild(i).GetComponent<Image>();

                        if (image == null)
                        {
                            Debugger.Instance.Error("ScrollableMenuManager", "O ícone de seleção de página na posição " + i + " está sem o componente de imagem");
                        }
                        _pageSelectionImages.Add(image);
                    }
                }
            }
        }

        private void SetPageSelection(int pageIndex)
        {
            // Se o indíce da página que haviamos selectionado anteriormente for igual ao indíce dá pagina que estamos selecionando agora, não fazer nada
            if (_previousPageSelectionIndex == pageIndex)
            {
                return;
            }

            // Deselecionar página anterior
            if (_previousPageSelectionIndex >= 0)
            {
                Debug.Log("PREVIOUS PAGE SELECTION INDEX: " + _previousPageSelectionIndex);

                _pageSelectionImages[_previousPageSelectionIndex].sprite = scrollableMenuSettings.unselectedPage;
                _pageSelectionImages[_previousPageSelectionIndex].SetNativeSize();
            }

            // Selecionar nova página
            _pageSelectionImages[pageIndex].sprite = scrollableMenuSettings.selectedPage;
            _pageSelectionImages[pageIndex].SetNativeSize();
            _previousPageSelectionIndex = pageIndex;
        }

        /// <summary>
        /// Faz o lerping para a próxima página
        /// </summary>
        private void NextScreen()
        {
            LerpToPage(_currentPage + 1);
        }

        /// <summary>
        /// Faz o lerping para a página anterior
        /// </summary>
        private void PreviousScreen()
        {
            LerpToPage(_currentPage - 1);
        }

        /// <summary>
        /// Retorna o indíce da página mais próxima
        /// </summary>
        /// <returns></returns>
        private int GetNearestPage()
        {
            //Com base na distância da posição atual, encontre a página mais próxima
            Vector2 currentPosition = _content.anchoredPosition;

            float distance = float.MaxValue;
            int nearestPage = _currentPage;

            for (int i = 0; i < _pagePositions.Count; i++)
            {
                float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
                if (testDist < distance)
                {
                    distance = testDist;
                    nearestPage = i;
                }
            }

            return nearestPage;
        }

        private void HandleOnBeginDrag(PointerEventData eventData)
        {
            // Se estiver em lerping, então pare enquanto o usuário está arrastando
            _isLerpInProgress = false;
            // Não arrastando ainda
            _isDragging = false;
        }

        private void HandleOnEndDrag(PointerEventData eventData)
        {
            //O quanto o conteúdo do container foi arrastado
            float dragContentDifference;

            if (scrollableMenuSettings.scrollRect.horizontal)
            {
                dragContentDifference = _startPosition.x - _content.anchoredPosition.x;
            }
            else
            {
                dragContentDifference = -(_startPosition.y - _content.anchoredPosition.y);
            }

            //Teste para deslizar rápido - deslizar que move apenas +/- 1 item
            if (Time.unscaledTime - _timeStamp < scrollableMenuSettings.fastSwipeThresholdTime &&
                Mathf.Abs(dragContentDifference) > scrollableMenuSettings.fastSwipeThresholdDistance &&
                Mathf.Abs(dragContentDifference) < _fastSwipeThresholdMaxLimit)
            {
                if (dragContentDifference > 0)
                {
                    NextScreen();
                }
                else
                {
                    PreviousScreen();
                }
            }
            else
            {
                //Faça o Lerping para qual página chegamos
                LerpToPage(GetNearestPage());
            }

            _isDragging = false;
        }

        private void HandleOnDrag(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                //Arrastando agora
                _isDragging = true;

                //Economizar tempo - sem escala, portanto, pausar com Time.scale não deve afetá-lo
                _timeStamp = Time.unscaledTime;

                //Salva a posição atual do container
                _startPosition = _content.anchoredPosition;
            }
            else
            {
                if (_isShowingPageSelection)
                {
                    SetPageSelection(GetNearestPage());
                }
            }
        }
    }
}
