using Microsoft.EntityFrameworkCore;
using SkillSwap.Seeders;

namespace SkillSwap.Models;
/* Class detailing the database conection and all the entities on it, also the conditions of the attributes before to create a registrer*/
public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Qualification> Qualifications { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StateRequest> StateRequests { get; set; }

    public virtual DbSet<StateUser> StateUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Ability> Abilities { get; set; }

    public virtual DbSet<Report> Reports { get; set; }
    public virtual DbSet<StateReport> StateReports { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        RoleSeeder.Seed(modelBuilder);
        StateRequestSeeder.Seed(modelBuilder);
        StateUserSeeder.Seed(modelBuilder);
        StateReportSeeder.Seed(modelBuilder);
        QualificationSeeder.Seed(modelBuilder);
        AbilitiesSeeder.Seed(modelBuilder);
        UserSeeder.Seed(modelBuilder);
        ReportSeeder.Seed(modelBuilder);
        RequestSeeder.Seed(modelBuilder);

        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<Qualification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AccumulatorAdition)
                .HasColumnType("int(11)")
                .HasColumnName("accumulator_adition");
            entity.Property(e => e.Count)
                .HasColumnType("int(11)")
                .HasColumnName("count");

            entity.HasOne(u => u.User)
                .WithOne(up => up.Qualification)
                .HasForeignKey<User>(up => up.IdQualification)
            .HasConstraintName("Qualification_CualificationProfile_FK");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.IdReceivingUser, "id_receiving_user");

            entity.HasIndex(e => e.IdRequestingUser, "id_requesting_user");

            entity.HasIndex(e => e.IdStateRequest, "id_state_request");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DisponibilitySchedule)
                .HasMaxLength(255)
                .HasColumnName("disponibility_schedule");
            entity.Property(e => e.IdReceivingUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_receiving_user");
            entity.Property(e => e.IdRequestingUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_requesting_user");
            entity.Property(e => e.IdStateRequest)
                .HasColumnType("int(11)")
                .HasColumnName("id_state_request");

            entity.HasOne(d => d.IdReceivingUserNavigation).WithMany(p => p.RequestIdReceivingUserNavigations)
                .HasForeignKey(d => d.IdReceivingUser)
                .HasConstraintName("Requests_ibfk_2");

            entity.HasOne(d => d.IdRequestingUserNavigation).WithMany(p => p.RequestIdRequestingUserNavigations)
                .HasForeignKey(d => d.IdRequestingUser)
                .HasConstraintName("Requests_ibfk_3");

            entity.HasOne(d => d.IdStateRequestNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.IdStateRequest)
                .HasConstraintName("Requests_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StateRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("State_requests");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StateUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("State_users");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DurationSuspension)
                .HasColumnType("int(11)")
                .HasColumnName("duration_suspension");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.IdRol, "id_rol");

            entity.HasIndex(e => e.IdState, "id_state");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IdQualification)
                .HasColumnType("int(11)")
                .HasColumnName("id_qualification");
            entity.Property(e => e.IdRol)
                .HasColumnType("int(11)")
                .HasColumnName("id_rol");
            entity.Property(e => e.IdState)
                .HasColumnType("int(11)")
                .HasColumnName("id_state");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(100)
                .HasColumnName("job_title");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(30)
                .HasColumnName("phone_number");
            entity.Property(e => e.PasswordResetToken)
                .HasMaxLength(512)
                .HasColumnName("password_reset_token");
            entity.Property(e => e.PasswordResetTokenExpiry)
                .HasMaxLength(65)
                .HasColumnName("password_reset_token_expiry");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.SuspensionDate).HasColumnName("suspension_date");
            entity.Property(e => e.ReactivationDate).HasColumnName("reactivation_date");
            entity.Property(e => e.UrlImage)
                .HasMaxLength(100)
                .HasColumnName("url_image");
            entity.Property(e => e.UrlLinkedin)
                .HasMaxLength(100)
                .HasColumnName("url_linkedin");
            entity.Property(e => e.UrlGithub)
                .HasMaxLength(100)
                .HasColumnName("url_github");
            entity.Property(e => e.UrlBehance)
                .HasMaxLength(100)
                .HasColumnName("url_behance");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("Users_ibfk_2");

            entity.HasOne(d => d.IdStateNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdState)
                .HasConstraintName("Users_ibfk_1");

            entity.HasOne(u => u.Ability)
                .WithOne(up => up.User)
                .HasForeignKey<User>(up => up.IdAbility)
            .HasConstraintName("Users_ibfk_3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
