using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
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
                    // 18, Because of this, the API can validate the value based on the DataAnnotation attributes declared inside VillaDTO.
                    // 18, There are some other features when using this attribute.
                    // 18, If we dont want to use this attribute -> goto [HttpPost]
    public class VillaAPIController : ControllerBase
    {
        //private readonly ILogger<VillaAPIController> _logger;

        //// 24, Dependency injecting logger which is already registered inside the CreateBuilder()
        //// 24, By using DI, we dont have to worry about instatiating or disposing the logger object.
        //public VillaAPIController(ILogger<VillaAPIController> logger)
        //{
        //    _logger = logger;
        //}

        // 27, How to create and inject your own logger using DI.
        // 27, Create an ILogging and Logging implementation inside Logging folders
        // 27, Inject the independency in program.cs (Singleton, Scoped, Transient)
        private readonly ILogging _logging;
        public VillaAPIController(ILogging logging)
        {
            _logging = logging;
        }

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
            // 24, Logger
            //_logger.LogInformation("Getting all the villas");

            // 27, Logging
            _logging.Log("Getting all the villas", "");

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
        // 17, Name property, the route name of this GET request, DOES NOT DEPEND ON ROUTE VALUES.
        [HttpGet("{id:int}", Name = "GetVilla")] // 12, Explicitly define this parameter is of type INTEGER.
                                                 // 12, https://localhost:7291/api/VillaAPI/1 <- diff. URI/URL.
                                                 // 12, Thus, the id param (Case-SENsitive) is now REQUIRED. Wrong casing cause 404 not found for valid and invalid id.
                                                 // 13, ActionResult<>
                                                 // 14, 400, 404 statuscode are returned but it is undocumented.
                                                 // 14, Thus, to document the possible returns of this endpoint by using [ProducesResponseType()]
        [ProducesResponseType(StatusCodes.Status200OK)]     // Under schema section, now there is a ProblemDetails schema
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // 14, [ProducesResponseType(200)]
        // 14, [ProducesResponseType(400)]
        // 14, [ProducesResponseType(404)]    
        //[ProducesResponseType(StatusCodes.Status200OK,Type = typeof(VillaDTO))]
        //public ActionResult GetVillas(int id)       // 15, Does not have a return type. Swagger now does not know about the return type of the endpoint.
        // 15, http://localhost:7291/api/VillaAPI/1 now it is not working (localhost didn’t send any data. ERR_EMPTY_RESPONSE)
        // 15, We can specifies the return type of the action in [ProducesResponseType()]
        public ActionResult<VillaDTO> GetVillas(int id)
        {
            if (id == 0)
            {
                // 25, Logger
                //_logger.LogError("Get villa error with the id: " + id);

                // 26, Logging
                _logging.Log("Get villa error with the id: " + id, "error");

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

            // 13, StatusCode.Status200OK
            return Ok(villa);

            // 10, This might returns NULL. However, it is OKAY.
            //return VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);    // return one record.
        }


        // 16, Enpoint(method,action) for creating a new villa
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]        // 17, Update status for CreateAtRoute()
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)   // 16, Typically, the object is received from body(content) of a request.
                                                                                  // 16, Using attribute [FromBody] to denote the source of object
        {
            // 18, If [APIController] is not used. We can use ModelState.IsValid (built-in with .net core)
            // 18, ModelState in this case is the VillaDTO model.
            // 18, Using debugger to view ModelState (what properties is not satisfied the requirement and what error)
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            // 19, if we use both [APIController] and model state, the constraint check will be process by the [APIController] before passing into Post endpoint (ModelState check will become redundant). 
            // 19, Making custom validation - UNIQUE villa name
            if (VillaStore.villaList.FirstOrDefault(villa => villa.Name == villaDTO.Name) != null)
            {
                // 19, Error key should be unique.
                ModelState.AddModelError("CustomError", "Villa is already exists");
                return BadRequest(ModelState);
            }

            if (villaDTO == null)
            {
                return BadRequest();    // 16, BadRequest(0 is from ControllerBase class
            }
            if (villaDTO.Id > 0)   // 16, This is not a create request. Id should not be a value
            {
                // 16, There is no method for InternalServerError. Return custom result that is not in the default action result like BadRequest()
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            // 16, Assume users will input a distinct ID.
            // 16, Retrieve maximum ID and increase by 1.
            // 16, Possible null.
            villaDTO.Id = VillaStore.villaList.OrderByDescending(villa => villa.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);
            // 16, Since we dont have  a DB to persist the change this will go away everytime restarting the application.

            //return Ok(villaDTO);
            // 17, Sometime API needs to return the added record as a URL to the users.
            // 17, Thus, we need to link to GET by id of the new resource.
            // 17, CreateAtRoute("RouteName", anon types for route values, return object. Anon type property name must be the same as param of GET request (case-insensitive). Wrong route value name cause 500 internal server error
            // 17, CreateAtRoute if success return 201. Return url to the created record.
            return CreatedAtRoute("GetVilla", new { iD = villaDTO.Id }, villaDTO);

            // 18, Use DataAnnotation for data validation inside VillaDTO
        }

        // 20, Create a delete request
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // 20, Since we dont need a return type (<VillaDTO>), delete dont return any data just NoContent() -> Can use IActionResult
        public IActionResult DeleteVilla(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            // 20, Can return Ok(). Should return NoContent().
            return NoContent();
        }

        // 21, Update request update all data of a record.
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            // 21, This request will receive both id and a record.
            if (villaDTO == null || id != villaDTO.Id)  // 21, Check if input id is the same in the record.
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDTO.Name;
            // 21, New properties are added to test PUT request.
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;

            return NoContent();

        }

        // 22, Update only 1 property
        // 22, Install JsonPatch, NewtonSoftJson package from Nuget PM to the project.
        // 22, JsonPatch, NewtonSoftJson version should be the same .NET version when creating the project.
        // 22, Include these in Program.cs builder.Services.AddController() -> builder.Services.AddController().NewtonSoftJson() -> PATCH support is added to the service
        // 22, https://jsonpatch.com/ - For more info on JsonPatch - How patch operation works
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
