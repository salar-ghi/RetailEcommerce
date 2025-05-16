namespace Infrastructure.EntityTypeConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired(false).HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.Property(u => u.FirstName).IsRequired(false).HasMaxLength(50);
        builder.Property(u => u.LastName).IsRequired(false).HasMaxLength(50);
        builder.Property(u => u.RefreshToken).IsRequired(false).HasMaxLength(500);
        builder.Property(u => u.RefreshTokenExpiryTime).IsRequired(false);
        builder.Property(u => u.ProfilePictureUrl).IsRequired(false).HasMaxLength(5000);

        builder.HasMany(u => u.Basket)
              .WithOne(b => b.User)
              .HasForeignKey(b => b.UserId).IsRequired(false);

        builder.HasOne(u => u.Supplier)
               .WithOne(b => b.User)
               .HasForeignKey<Supplier>(s => s.UserId); // Assuming Supplier has a one-to-one with User

        builder.HasMany(u => u.UserRoles)
               .WithOne(r => r.User)
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Addresses)
               .WithOne(a => a.User)
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Reviewer)
               .WithOne(a => a.User)
               .HasForeignKey(a => a.UserId);

        builder.HasMany(u => u.Orders)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id).ValueGeneratedOnAdd();
        builder.Property(ur => ur.Name).IsRequired().HasMaxLength(50);
        builder.HasMany(u => u.UserRoles)
               .WithOne(r => r.Role)
               .HasForeignKey(r => r.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
        builder.Property(ur => ur.Id).ValueGeneratedOnAdd();
        builder.HasOne(ur => ur.User)
        .WithMany(u => u.UserRoles)
        .HasForeignKey(ur => ur.UserId);
        builder.HasOne(ur => ur.Role)
        .WithMany(r => r.UserRoles)
        .HasForeignKey(ur => ur.RoleId);
    }
}

public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.HasKey(ua => ua.Id);
        builder.Property(ua => ua.Id).ValueGeneratedOnAdd();
        builder.Property(ua => ua.Street).HasMaxLength(200);
        builder.Property(ua => ua.City).HasMaxLength(100);
        builder.Property(ua => ua.State).HasMaxLength(100);
        builder.Property(ua => ua.ZipCode).HasMaxLength(20);
        builder.Property(ua => ua.Country).HasMaxLength(100);
    }
}