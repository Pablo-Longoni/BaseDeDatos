using Postgrest.Models;
using Postgrest.Attributes;
using System.Collections.Generic;

public class trivia : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    //Mapea la propiedad category con la columna Category en la bd
    [Column("Category")]
    //Guarda el nombre de la trivia
    public string category { get; set; }

    // Lista de preguntas asociadas a esta trivia
    public List<question> questions { get; set; }
}

