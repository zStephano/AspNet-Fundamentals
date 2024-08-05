using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Requests.Categories
{
    public class EditorCategoryViewModel
    {
        [Required]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre 3 e 40 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O Slug é obrigatório")]
        public string Slug { get; set; }
    }
}
