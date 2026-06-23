using CustomCodeFramework.Postgres.EntityFramework.Configurations;
using Dhole.AuditLogs.Domain.AuditEvents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dhole.AuditLogs.Persistence.Configurations.AuditEvents;

internal sealed class AuditEventConfiguration : EntityTypeConfigurationBase<AuditEvent, Guid>
{
    public override void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        base.Configure(builder);

        builder.ToTable("AuditEvents");

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.EventId).IsRequired();

        builder.Property(x => x.CorrelationId).IsRequired();

        builder.Property(x => x.SourceService).HasMaxLength(120).IsRequired();

        builder.Property(x => x.EntityType).HasMaxLength(160);

        builder.Property(x => x.EntityId);

        builder.Property(x => x.Action).HasMaxLength(120).IsRequired();

        builder.Property(x => x.EventType).HasMaxLength(220);

        builder.Property(x => x.UserId);

        builder.Property(x => x.UserName).HasMaxLength(250);

        builder.Property(x => x.IpAddress).HasMaxLength(80);

        builder.Property(x => x.UserAgent).HasMaxLength(1000);

        builder.Property(x => x.OccurredAt).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.BeforeJson).HasColumnType("jsonb");

        builder.Property(x => x.AfterJson).HasColumnType("jsonb");

        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");

        builder.Property(x => x.MetadataJson).HasColumnName("Metadata").HasColumnType("jsonb");

        builder.Property(x => x.DetailsJson).HasColumnName("Details").HasColumnType("jsonb");

        builder.Property(x => x.ErrorMessage).HasMaxLength(4000);

        builder.Property(x => x.StackTrace).HasColumnType("text");

        builder.Ignore(x => x.Details);

        builder.HasIndex(x => x.EventId).IsUnique();

        builder.HasIndex(x => x.CorrelationId);

        builder.HasIndex(x => new { x.SourceService, x.OccurredAt });

        builder.HasIndex(x => new { x.EntityType, x.EntityId });

        builder.HasIndex(x => new { x.UserId, x.OccurredAt });

        builder.HasIndex(x => x.EventType);

        builder.HasIndex(x => x.Action);

        builder.HasIndex(x => x.OccurredAt);
    }
}
