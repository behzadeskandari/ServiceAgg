using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Models;
namespace ServiceAggregator.Api.Data;
public class ServiceAggregatorContext : DbContext
{
    public ServiceAggregatorContext(DbContextOptions<ServiceAggregatorContext> options)
        : base(options)
    {
    }

    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceMapping> ServiceMappings => Set<ServiceMapping>();
    public DbSet<Aggregate> Aggregates => Set<Aggregate>();
    public DbSet<AggregateStep> AggregateSteps => Set<AggregateStep>();
    public DbSet<AggregateStepMapping> AggregateStepMappings => Set<AggregateStepMapping>();
    public DbSet<AggregateMapping> AggregateMappings => Set<AggregateMapping>();
    public DbSet<ExecutionLog> ExecutionLogs => Set<ExecutionLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AggregateMapping>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.AggregateId)
                .HasDatabaseName("IX_AggregateMappings_AggregateId");
        });

        modelBuilder.Entity<AggregateStep>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.AggregateId)
                .HasDatabaseName("IX_AggregateStep_AggregateId");

            entity.HasIndex(x => x.ServiceId)
                .HasDatabaseName("IX_AggregateSteps_ServiceId");

            entity.HasIndex(x => new { x.AggregateId, x.Id })
                .HasDatabaseName("IX_AggregateSteps_AggregateId_Id");

            entity.Property(e => e.Status)
                  .HasDefaultValue(true);

            entity.HasOne(e => e.Service)
                  .WithMany()
                  .HasForeignKey(e => e.ServiceId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Mappings)
                  .WithOne()
                  .HasForeignKey(m => m.AggregateStepId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.PositionX)
                  .HasPrecision(18, 13);

            entity.Property(e => e.PositionY)
                  .HasPrecision(18, 13);
        });

        modelBuilder.Entity<AggregateStepMapping>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.AggregateStepId)
                .HasDatabaseName("IX_AggregateStepMapping_AggregateStepId");

            entity.Property(e => e.Source)
                  .HasMaxLength(50)
                  .HasDefaultValue("AggregateInput");

            entity.Property(e => e.TargetField)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.SourceField)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.Status)
                  .HasDefaultValue(true);
        });

        modelBuilder.Entity<Aggregate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(a => a.CreateDate)
                .HasDatabaseName("IX_Aggregates_CreateDate");
            
            entity.Property(e => e.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.Description)
                  .HasMaxLength(500);

            entity.Property(e => e.Status)
                  .HasDefaultValue(true);

            entity.HasMany(e => e.Steps)
                  .WithOne()
                  .HasForeignKey(m => m.AggregateId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Mappings)
                  .WithOne()
                  .HasForeignKey(m => m.AggregateId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.Url)
                  .HasMaxLength(500)
                  .IsRequired();

            entity.Property(e => e.Method)
                  .HasMaxLength(10)
                  .HasDefaultValue("POST");

            entity.Property(e => e.Type)
                  .HasMaxLength(20)
                  .HasDefaultValue("REST");

            entity.Property(e => e.Status)
                  .HasDefaultValue(true);

            entity.HasMany(e => e.Mappings)
                      .WithOne()
                      .HasForeignKey(m => m.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ServiceMapping>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.ServiceId)
                .HasDatabaseName("IX_ServiceMapping_ServiceId");

            entity.Property(e => e.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.Direction)
                  .IsRequired();

            entity.Property(e => e.Type)
                  .IsRequired();
        });
        modelBuilder.Entity<ExecutionLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TrackerId)
                .HasDatabaseName("IX_ExecutionLog_TrackerId");
        });
    }
}
