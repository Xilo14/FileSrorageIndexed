using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSIBackend.Models {
    public class LemmaEntry {
        public enum FieldEntriesEnum {
            OriginalName,
            Description,
            Content,
            ContentType,
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LemmaEntryId { get; set; }
        public int Count { get; set; }

        public FieldEntriesEnum FieldEntry { get; set; }

        public int FileId { get; set; }
        public File File { get; set; }

        public int LemmaId { get; set; }
        public Lemma Lemma { get; set; }

    }
}
