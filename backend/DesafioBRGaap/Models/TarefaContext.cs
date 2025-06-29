using Microsoft.EntityFrameworkCore;

namespace DesafioBRGaap.Models
{
    public class TarefaContext : DbContext
    {
        public TarefaContext(DbContextOptions<TarefaContext> options) : base(options)
        {
        }

        public DbSet<Tarefa> Tarefas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(500);
                entity.Property(e => e.UserId)
                      .IsRequired();
                entity.Property(e => e.Completed)
                      .IsRequired();

                entity.HasIndex(e => e.Title);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}