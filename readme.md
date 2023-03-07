[![semantic-release](https://img.shields.io/badge/%20%20%F0%9F%93%A6%F0%9F%9A%80-semantic--release-e10079.svg)](https://github.com/semantic-release/semantic-release)

# Introdução

ScrollableMenuSystem é um menu com ScrollRect criado com intuito de aperfeiçoar a experiência do usuário em navegação de telas. 

## Como utilizar
Ao importar o ScrollableMenuSystem, você terá em sua pasta um prefab chamado `ScrollableMenuPanel`, e este prefab precisar ser adicionado à um Canvas para funcionar corretamente. 

![](https://ibb.co/tL5CFHb)

E para usar este pacote também é preciso seguir 2 passos. Em primeiro lugar, você precisa adicionar uma tela para dentro do container localizado dentro de `ScrollMap`. Essa tela é uma das telas que farão parte do conjunto do seu menu.

Observação: A tela que será adicionado ao container precisa possuir o Width e Height com o mesmo tamanho definido em `ScrollMap`. No nosso exemplo, a tela que usamos tem uma largura e altura de 1080x1920 (FullHD Portrait), pois o `ScrollMap` possui essas dimensões. Segue o Exemplo:

![](https://ibb.co/M2tMsS6)

Pronto! Agora que adicionamos as nossas telas, só precisamos fazer a segunda coisa que é adicionar os botões de navegação!
Os botões de navegação são adicionados dentro do container localizado em `NavegationPanel`. Segue o exemplo:

![](https://ibb.co/1mNdf9h)

Atenção: Agora que concluimos os 2 passos, é importante considerar que a quantidade de telas precisa ser EXATAMENTE a mesma que a quantidade de botões de navegação, pois eles serão associados dinamicamente via código (Ou seja, não precisamos lincar os botões e/ou as telas um a um).

O Inspector precisa de uma atenção especial nossa. Este pacote possui o ScrollableMenuManager que gerencia a pilha completa de botões, telas/páginas, rectTransforms, swipe, etc. Para conseguirmos usar os botões de navegação (é possível navegar sem os botões de navegação), precisaremos APENAS associar o container que possui os botões de navegação na variável `PageSelectionContainer` que está serializada. Segue o exemplo:

![](https://ibb.co/QJX3YWV)


Perfeito! Tudo deve estar funcionando perfeitamente agora. Tente customizar a vontade os menus e os botões de navegação.

## Convenção de Package Name
A documetnação do Unity recomenda uma convensão especifica para nomes de packages.:

> Comece com `com.<company-name>`. Por exemplo, um dos pacotes oficiais do Unity é `com.unity.timeline`. 

Para mais detalhes acesse a [documentação](https://docs.unity3d.com/2020.1/Documentation/Manual/cus-naming.html).

Com isso em mente, todos os pacotes da homy para UPM devem seguir o padrão. `com.hgs.my-package-name`. Por exemplo, este template é `com.hgs.upm-template`.

Este nome deve ser especificado em `name` no **package.json**.

**ATENÇÃO** O nome do package junto e nome do repositório não podem ser alterados! Caso isso aconteça outros packages ou projetos perderão a referencia.

Além do campo `name` existe outro campo chamado `displayName` este pode ser alterado sempre que necessário, este nome aparecerá na janela do Unity Package Manager.

## Convenção de namespace
Para isolar os assets de outros scripts isolamos todos no namespace do package `HGS.<package-name>`. Por exemplo, neste package de template usamos `HGS.Template`.

## Convenção de Assembly
Cada pasta na raiz do package precisa de um AssemblyDefinition, por tanto utilizamos a convenção `HGS.<pacakge-name>.<folder-name>`. Por exemplo, neste projeto possuirmos a pasta Runtime, onde o Assembly é `HGS.Template.Runtime`.

## Branchs
Todos os packages devem possuir duas branchs reservadas.:

- `master` -> Aqui guardamos todo material do projeto.
- `upm` -> Aqui mantemos uma copia do package que se encontra na pasta `Assets/Package`.

Sempre que um merge é feito na branch `unity`, o script de CI  irá criar uma copia da subpasta `Assets/Package` automaticamente na branch `upm`. Portanto é importante que exista uma pasta chamada `Package` dentro de `Assets` para o deploy ocorra com sucesso. 

## Alterando versões de um package
Utilizamos o plugin [semantic-release](https://github.com/semantic-release/semantic-release) para facilitar o sistema de release e versionamento, portanto, sempre inicie um repositorio na versão 0.0.0, pois este será alterado automaticamente conforme o uso.

Para utilizar o semantic-relase, temos utilizar a seguinte convenção se commits.:

```
<type>(<scope>): <short summary>
  │       │             │
  │       │             └─⫸ Summary in present tense. Not capitalized. No period at the end.
  │       │
  │       └─⫸ Commit Scope: Namespace, script name, etc..
  │
  └─⫸ Commit Type: build|ci|docs|feat|fix|perf|refactor|test
```

`Type`.: 

- build: Changes that affect the build system or external dependencies (example scopes: package system)
- ci: Changes to our CI configuration files and scripts (example scopes: Circle, - BrowserStack, SauceLabs)
- docs: Documentation only changes
- feat: A new feature
- fix: A bug fix
- perf: A code change that improves performance
- refactor: A code change that neither fixes a bug nor adds a feature
- test: Adding missing tests or correcting existing tests

### Observação

Certifique-se de exlcuir todos os meta files caso você copie os arquivos deste repositório e cole em outro lugar, isso evita conflito de meta no unity.