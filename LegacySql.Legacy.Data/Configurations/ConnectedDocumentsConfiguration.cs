using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ConnectedDocumentsConfiguration : IEntityTypeConfiguration<ConnectedDocumentsEF>
    {
        public void Configure(EntityTypeBuilder<ConnectedDocumentsEF> builder)
        {
            builder.HasIndex(e => new { e.Doc2Id, e.Type2 })
                .HasDatabaseName("connected_documents_doc2ID_type2");

            builder.HasIndex(e => new { e.Type1, e.Doc1Id })
                .HasDatabaseName("connected_documents_type1_doc1");

            builder.HasIndex(e => new { e.Doc1Id, e.Doc2Id, e.Type1, e.Type2 })
                .HasDatabaseName("connected_documents_type1_type2");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Doc1Id).HasColumnName("doc1ID");

            builder.Property(e => e.Doc2Id).HasColumnName("doc2ID");

            builder.Property(e => e.Type1).HasColumnName("type1");

            builder.Property(e => e.Type2).HasColumnName("type2");
            builder.Property(e => e.Date).HasColumnName("zt");
        }
    }
}
