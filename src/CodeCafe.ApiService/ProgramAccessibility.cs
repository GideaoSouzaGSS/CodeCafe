using System.Runtime.CompilerServices;

// Tornar os tipos internos visíveis para os projetos de teste
[assembly: InternalsVisibleTo("CodeCafe.Tests.Integration")]
[assembly: InternalsVisibleTo("CodeCafe.Tests.E2E")]
[assembly: InternalsVisibleTo("CodeCafe.Tests.Unit")]
[assembly: InternalsVisibleTo("CodeCafe.Tests.CodeCafeIntegration")]
[assembly: InternalsVisibleTo("CodeCafe.Tests.CodeCafeE2E")]
[assembly: InternalsVisibleTo("CodeCafe.Tests.CodeCafeUnit")]
[assembly: InternalsVisibleTo("CodeCafe.Tests.CodeCafeUI")]

namespace CodeCafe.ApiService
{
    // Classe parcial pública que permite o acesso ao Program
    public partial class Program { }
}