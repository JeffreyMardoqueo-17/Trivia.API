using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{
    public class StartGameSessionResult
    {
        //esta no es de la bd es solo algo que devuelve el sp de la sesion es para mostrar el resultado (aqui se peude ver mejor en el service)
        public int GameSessionId { get; set; }
    }
}