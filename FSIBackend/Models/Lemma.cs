using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSIBackend.Models {
    public class Lemma {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LemmaId { get; set; }
        public string Value { get; set; }

    }
}
