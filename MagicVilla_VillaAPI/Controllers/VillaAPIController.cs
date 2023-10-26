using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc; // contains ControllerBase class

namespace MagicVilla_VillaAPI.Controllers
{
    // ControllerBase contains methods for returning all the data and users that are related to the controller .NET Application.
    // If an MVC application is used, the controller class will derived from Controller class rather than ControllerBase class.
    // Controller class supports Views which are used in an MVC application 
    // Since this application is an API app. thus adding support for the MVC views will be an overhead.
    // Using ControllerBase to reduce overhead. Can be upgraded later on by changing the inherited class
// CONTROLLER LEVEL
    //[Route("api/[controller]")] // 7, Using [controller] will automaticaly change the API ROUTE for all of the other clients, whenever the controller class's name (VillaAPI) change. Thus, we have to notifies the CONSUMERS**?. For that reason hard-coded API route is better.? 
    [Route("api/VillaAPI")] // 4, Action methods on controllers annotated with ApiControllerAttribute must be attribute routed.
    [ApiController] // 1, This attribute notifies the controller that this will be an API controller
    public class VillaAPIController: ControllerBase
    {
        // Base class for the controller

        // 2, An API typically returns data (Models(Class)) -> create a new folder called Models

        // 3, Create an ENDPOINT**? -> Need attribute [Route] on top of the class. - Action methods on controllers annotated with ApiControllerAttribute must be attribute routed.

        // ACTION METHOD LEVEL
        // Fetch error response status is 500 https://localhost:7291/swagger/v1/swagger.json
        /*[HttpGet] // 5, When adding an endpoint to the API controller, an http verb (GET, POST, ...) must be defined for that endpoint
                  // 5, Since this enpoint(action method) retrieve all the villas, an HTTP GET endpoint attribute will be defined for this endpoint(action method).
                  // 5, This will notifies the SWAGGER documentation (doc.) that this endpoint is a get endpoint 
        public IEnumerable<Villa> GetVillas()
        {
            return new List<Villa> {
            new Villa{Id=1,Name="Pool View" },
            new Villa{Id=2,Name="Beach View" }
            };
        }*/

        // 8, IRL, API controller doesnt use directly Models, because it is not desireable in PRODUCTION APPLICATION.
        // 8, Thus, DTO is used - DTO provides a wrapper between the entity and the DB model and what is being exposed from the API.
        /*[HttpGet]
        public IEnumerable<VillaDTO> GetVillas()
        {
            return new List<VillaDTO> {
            new VillaDTO{Id=1,Name="Pool View" },
            new VillaDTO{Id=2,Name="Beach View" }
            };
        }*/

        // 6, SWAGGER - Doesnt need POSTMAN -- API can be tested directly on SWAGGER
        // 6, Swagger UI is a fancy doc. where it displays all the endpoints of the API to test, clear, execute again

        // 9, API is typically used to perform CRUD operations - create, read, update, delete VillaDTO. Usually, a DB will store all of the info.. However, to simplify, we create on-the-fly data store which is the Data folder which includes VillaStore static class 
        [HttpGet]
        // 13, When working with API, status code should be return from all of the endpoints. 
        // 13, One way to return status code is to declare the return type as ActionResult<> which implement IActionResult interface(NOT ENTIRELY TRUE) 
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            // 13, Return an OkOjectResult object that produces a Statuscodes.Status200OK response.
            return Ok(VillaStore.villaList);    // return multiple records
            
            //return VillaStore.villaList;    // return multiple records
        }

        // 10, Get villa base on ID
        //[HttpGet] // 10, 5, If an HTTP verb is not declare the [HttpGet] will be the default attribute of the endpoint(method, action)**.
        // 10, However, this will throw AmbigousMatchException: The request matched multiple endpoints.(at https://localhost:7291/api/VillaAPI), since [HttpGet] causes confusion on whether which endpoint should be invoked.  
        //[HttpGet("id")] // 11, If the attribute expects a parameter, EXPLICITLY DEFINE it as the parameter of the attribute
                        // 11, https://localhost:7291/api/VillaAPI/id?id=1 <- for id == 1
                        // 11, https://localhost:7291/api/VillaAPI/id <- for default (No input, id is NOT REQUIRED)
        [HttpGet("{id:int}")] // 12, Explicitly define this parameter is of type INTEGER.
                              // 12, https://localhost:7291/api/VillaAPI/1 <- diff. URI/URL.
                              // 12, Thus, the id param is now REQUIRED.
        // 13, ActionResult<>
        public ActionResult<VillaDTO> GetVillas(int id)
        {
            if (id == 0)
            {
                // 13, StatusCodes.Status400BadRequest response if id == 0
                return BadRequest();
            }
            // 13, Store query result in a villa
            var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);
            if (villa == null)
            {
                // 13, StatusCodes.Status404NotFound response if villa == null
                return NotFound();
            }

            // 13, 
            return Ok(villa);

            // 10, This might returns NULL. However, it is OKAY.
            //return VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);    // return one record.
        }
    }
}
