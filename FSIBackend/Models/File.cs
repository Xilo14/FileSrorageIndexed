using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSIBackend.Models {
    public class File {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileId { get; set; }
        public string OriginalName { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string FileExtension { get; set; }
        public string ContentType { get; set; }

    }
}
