using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCSA_Movies.Arashi256.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3, ErrorMessage = "The Title must be between 3 and 60 characters.")]
        [Required(ErrorMessage = "The Title is required.")]
        public string? Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date, ErrorMessage = "The Release Date must be a valid date.")]
        [Required(ErrorMessage = "The Release Date is required.")]
        public DateTime ReleaseDate { get; set; }

        [Range(1, 100, ErrorMessage = "The Price must be between 1 and 100.")]
        [DataType(DataType.Currency, ErrorMessage = "The Price must be a valid currency.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "The Price is required.")]
        public decimal Price { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s-]*$", ErrorMessage = "The Genre must start with a capital letter and only contain letters, spaces, and dashes.")]
        [Required(ErrorMessage = "The Genre is required.")]
        [StringLength(30, ErrorMessage = "The Genre must not exceed 30 characters.")]
        public string? Genre { get; set; }


        [Range(0.0, 10.0, ErrorMessage = "The Rating must be between 0.0 and 10.0.")]
        [Required(ErrorMessage = "The Rating is required.")]
        [Column(TypeName = "decimal(3, 1)")]
        [DisplayFormat(DataFormatString = "{0:0.#}", ApplyFormatInEditMode = false)]
        public decimal? Rating { get; set; }

        public override string ToString()
        {
            return $"Movie [Id={Id}, Title=\"{Title}\", ReleaseDate={ReleaseDate:yyyy-MM-dd}, Price={Price:C}, Genre=\"{Genre}\", Rating={Rating:0.0}]";
        }
    }
}