namespace Infrastructure.EntityTypeConfiguration;

public class FinanceTenantConfiguration : IEntityTypeConfiguration<FinanceTenant>
{
    public void Configure(EntityTypeBuilder<FinanceTenant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(40).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public class FinanceBranchConfiguration : IEntityTypeConfiguration<FinanceBranch>
{
    public void Configure(EntityTypeBuilder<FinanceBranch> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(40).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.HasOne(x => x.Tenant).WithMany(x => x.Branches).HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.HasOne(x => x.Branch).WithMany(x => x.CostCenters).HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ParentCostCenter).WithMany().HasForeignKey(x => x.ParentCostCenterId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ChartOfAccountConfiguration : IEntityTypeConfiguration<ChartOfAccount>
{
    public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.HasOne(x => x.ParentAccount).WithMany(x => x.Children).HasForeignKey(x => x.ParentAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class FinanceAccountConfiguration : IEntityTypeConfiguration<FinanceAccount>
{
    public void Configure(EntityTypeBuilder<FinanceAccount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CurrentBalance).HasColumnType("decimal(18,2)");
        builder.Property(x => x.BankName).HasMaxLength(120);
        builder.Property(x => x.AccountNumberMasked).HasMaxLength(80);
        builder.HasOne(x => x.Branch).WithMany(x => x.FinanceAccounts).HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.LedgerAccount).WithMany().HasForeignKey(x => x.LedgerAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class AccountingPeriodConfiguration : IEntityTypeConfiguration<AccountingPeriod>
{
    public void Configure(EntityTypeBuilder<AccountingPeriod> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.StartsOn, x.EndsOn }).IsUnique();
    }
}

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.JournalNumber).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => new { x.TenantId, x.JournalNumber }).IsUnique();
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReversalOfJournalEntry).WithMany().HasForeignKey(x => x.ReversalOfJournalEntryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DebitAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CreditAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ExchangeRateToBase).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasOne(x => x.JournalEntry).WithMany(x => x.Lines).HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.LedgerAccount).WithMany(x => x.JournalEntryLines).HasForeignKey(x => x.LedgerAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CostCenter).WithMany().HasForeignKey(x => x.CostCenterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class FinancialTransactionConfiguration : IEntityTypeConfiguration<FinancialTransaction>
{
    public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Category).HasMaxLength(120);
        builder.Property(x => x.CounterpartyName).HasMaxLength(250);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.HasOne(x => x.FinanceAccount).WithMany().HasForeignKey(x => x.FinanceAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CostCenter).WithMany().HasForeignKey(x => x.CostCenterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.JournalEntry).WithMany().HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class RecurringFinancialObligationConfiguration : IEntityTypeConfiguration<RecurringFinancialObligation>
{
    public void Configure(EntityTypeBuilder<RecurringFinancialObligation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.VendorName).HasMaxLength(250);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FinanceAccount).WithMany().HasForeignKey(x => x.FinanceAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    public void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PeriodCode).HasMaxLength(40).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.PeriodCode, x.BranchId });
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.JournalEntry).WithMany().HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PayrollLineConfiguration : IEntityTypeConfiguration<PayrollLine>
{
    public void Configure(EntityTypeBuilder<PayrollLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeId).HasMaxLength(80).IsRequired();
        builder.Property(x => x.EmployeeName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Position).HasMaxLength(120);
        builder.Property(x => x.BaseSalary).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Bonus).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Deductions).HasColumnType("decimal(18,2)");
        builder.Property(x => x.TaxWithheld).HasColumnType("decimal(18,2)");
        builder.Property(x => x.InsuranceWithheld).HasColumnType("decimal(18,2)");
        builder.Property(x => x.NetPay).HasColumnType("decimal(18,2)");
        builder.HasOne(x => x.PayrollRun).WithMany(x => x.Lines).HasForeignKey(x => x.PayrollRunId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class FinancialApprovalLogConfiguration : IEntityTypeConfiguration<FinancialApprovalLog>
{
    public void Configure(EntityTypeBuilder<FinancialApprovalLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntityName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(80).IsRequired();
        builder.Property(x => x.ApproverUserId).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.Property(x => x.AmountThreshold).HasColumnType("decimal(18,2)");
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.EntityId });
    }
}

public class FinancialAuditLogConfiguration : IEntityTypeConfiguration<FinancialAuditLog>
{
    public void Configure(EntityTypeBuilder<FinancialAuditLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntityName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Action).HasMaxLength(80).IsRequired();
        builder.Property(x => x.BeforeJson).HasColumnType("nvarchar(max)");
        builder.Property(x => x.AfterJson).HasColumnType("nvarchar(max)");
        builder.Property(x => x.CorrelationId).HasMaxLength(120);
        builder.Property(x => x.IpAddress).HasMaxLength(80);
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.EntityId, x.CreatedTime });
    }
}
