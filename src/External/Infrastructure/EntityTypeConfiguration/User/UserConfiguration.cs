namespace Infrastructure.EntityTypeConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.HasOne(u => u.Basket)
              .WithOne(b => b.User)
              .HasForeignKey<Basket>(b => b.UserId)
              .IsRequired(false); // Optional relationship

        builder.HasOne(u => u.Supplier)
               .WithOne()
               .HasForeignKey<Supplier>(s => s.Id); // Assuming Supplier has a one-to-one with User

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