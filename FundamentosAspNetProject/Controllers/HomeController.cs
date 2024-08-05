using FundamentosAspNetProject.Data;
using FundamentosAspNetProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace FundamentosAspNetProject.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Get([FromServices] DataContext context)
            => Ok(context.Todos.ToList());
        

        [HttpGet]
        [Route("/{id:int}")]
        public IActionResult GetById(
            [FromRoute] int id,
            [FromServices] DataContext context)
        {
            var todos = context.Todos.FirstOrDefault(x => x.Id == id);

            if(todos == null)
                return NotFound();

            return Ok(todos);
        }

        [HttpPost]
        [Route("/")]
        public IActionResult Post(
            TodoModel todo, 
            [FromServices] DataContext context)
        {
            context.Todos.Add(todo);
            context.SaveChanges();
            
            return Created($"/{todo.Id}", todo);
        }

        [HttpPut]
        [Route("/{id:int}")]
        public IActionResult Put(
            [FromRoute] int id,
            [FromBody] TodoModel todo,
            [FromServices] DataContext context)
        {
            var model = context.Todos.FirstOrDefault(x => x.Id == id);

            if (model == null)
                return NotFound();
            
            model.Title = todo.Title;
            model.Done = todo.Done;

            context.Todos.Update(todo);
            context.SaveChanges();

            return Ok(todo);
        }

        [HttpDelete]
        [Route("/{id:int}")]
        public IActionResult Delete(
            [FromRoute] int id,
            [FromBody] TodoModel todo,
            [FromServices] DataContext context)
        {
            var model = context.Todos.FirstOrDefault(x => x.Id == id);

            if (model == null)
                return NotFound();

            context.Todos.Remove(todo);
            context.SaveChanges();

            return Ok(todo);
        }
    }
}
