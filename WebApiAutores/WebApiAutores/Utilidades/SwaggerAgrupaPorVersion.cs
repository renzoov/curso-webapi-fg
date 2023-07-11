using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Utilidades
{
  public class SwaggerAgrupaPorVersion : IControllerModelConvention
  {
    public void Apply(ControllerModel controller)
    {
      var namespaceController = controller.ControllerType.Namespace; // Controller.V1
      var versionAPI = namespaceController.Split('.').Last().ToLower(); // v1
      controller.ApiExplorer.GroupName = versionAPI;
    }
  }
}
