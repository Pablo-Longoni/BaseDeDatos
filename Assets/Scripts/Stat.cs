using Postgrest.Models;
using Postgrest.Attributes;
using System;

public class Stat : BaseModel
{
    [PrimaryKey("id", false)] 
    public int id { get; set; }

    [Column("usuarios_id")]
    public string usuarios_id { get; set; }

    [Column("trivia_id")]
    public int trivia_id { get; set; }

    [Column("points")]
    public float points { get; set; }

    [Column("questions_answered")]
    public int questions_answered { get; set; }

}