# Introdução

ScrollableMenuSystem é um menu com ScrollRect criado com intuito de aperfeiçoar a experiência do usuário em navegação de telas. 

## Como utilizar
Ao importar o ScrollableMenuSystem, você terá em sua pasta um prefab chamado `ScrollableMenuPanel`, e este prefab precisar ser adicionado à um Canvas para funcionar corretamente. 

![](https://i.ibb.co/TW5hdwH/Scroller-Panel-Prefab.png)

E para usar este pacote também é preciso seguir 2 passos. Em primeiro lugar, você precisa adicionar uma tela para dentro do container localizado dentro de `ScrollMap`. Essa tela é uma das telas que farão parte do conjunto do seu menu.

Observação: A tela que será adicionado ao container precisa possuir o Width e Height com o mesmo tamanho definido em `ScrollMap`. No nosso exemplo, a tela que usamos tem uma largura e altura de 1080x1920 (FullHD Portrait), pois o `ScrollMap` possui essas dimensões. Segue o Exemplo:

![](https://i.ibb.co/PmPwNjQ/Scroll-Map.png)

Pronto! Agora que adicionamos as nossas telas, só precisamos fazer a segunda coisa que é adicionar os botões de navegação!
Os botões de navegação são adicionados dentro do container localizado em `NavegationPanel`. Segue o exemplo:

![](https://i.ibb.co/5R3nW5t/Nav.png)

Atenção: Agora que concluimos os 2 passos, é importante considerar que a quantidade de telas precisa ser EXATAMENTE a mesma que a quantidade de botões de navegação, pois eles serão associados dinamicamente via código (Ou seja, não precisamos lincar os botões e/ou as telas um a um).

O Inspector precisa de uma atenção especial nossa. Este pacote possui o ScrollableMenuManager que gerencia a pilha completa de botões, telas/páginas, rectTransforms, swipe, etc. Para conseguirmos usar os botões de navegação (é possível navegar sem os botões de navegação), precisaremos APENAS associar o container que possui os botões de navegação na variável `PageSelectionContainer` que está serializada. Segue o exemplo:

![](https://i.ibb.co/mRSVhnK/Page-Selection-Container.png)


Perfeito! Tudo deve estar funcionando perfeitamente agora. Tente customizar a vontade os menus e os botões de navegação.