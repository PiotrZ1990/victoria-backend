using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class StudentEditViewModel : StudentCreateViewModel
{
    public int Id { get; set; }
}
