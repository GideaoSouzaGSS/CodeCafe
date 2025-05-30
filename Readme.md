# CodeCafe

## Visão Geral

O CodeCafe é uma aplicação modular desenvolvida em .NET 9, focada em mensageria, APIs modernas, autenticação, armazenamento em nuvem e cache distribuído. O projeto serve como referência para boas práticas de arquitetura backend.(Veja o Readme do projeto de codigo em src/)

## Integração Contínua (GitHub Actions)

O projeto utiliza **GitHub Actions** para automação de build, testes e análise de cobertura.  
O workflow é disparado a cada push ou pull request para os branches principais e executa as seguintes etapas:

- **Checkout do código**
- **Setup do .NET**
- **Restore de dependências**
- **Build da solução**
- **Execução dos testes automatizados**
- **Geração e publicação do relatório de cobertura**
- **(Opcional) Deploy ou publicação de artefatos**

O arquivo de configuração do workflow está localizado em `.github/workflows/`.

---

Para dúvidas ou contribuições, consulte os arquivos de cada módulo ou abra uma issue.