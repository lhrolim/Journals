using journals.commons.Util;
using NHibernate.Mapping.Attributes;

namespace journals.commons.Model.Entities {

    [Class(Table = "PUBLICATION_DATA")]
    public class PublicationData : ABaseEntity
    {

        public const string ByPublication = "from PublicationData where Publication.id = ?";

        [Property(Type = "BinaryBlob", Column = "BinaryContent")]
        public virtual byte[] BinaryContent {
            get; set;
        }

        public virtual byte[] UnCompressed () => BinaryContent == null ? null : CompressionUtil.Decompress(BinaryContent);


        [ManyToOne(Lazy = Laziness.False,  Column = "publication_id", Unique = true, Cascade = "none")]
        public virtual Publication Publication {
            get; set;
        }


        public virtual string Base64String {
            get; set;
        }



    }
}
