using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anfx.Pasivos.Domain.Entities;
public class RSP_Input
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [MaxLength(40)]
    public string NomInput { get; set; } = "";


}
