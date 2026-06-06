using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260606000000_AddFinanceModule")]
    public partial class AddFinanceModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinanceTenants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    BaseCurrency = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceTenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChartOfAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    NormalBalance = table.Column<int>(type: "int", nullable: false),
                    ParentAccountId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsPostingAllowed = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemAccount = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartOfAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartOfAccounts_ChartOfAccounts_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountingPeriods",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    StartsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    ClosedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinanceBranches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManagerUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanceBranches_FinanceTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "FinanceTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostCenters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParentCostCenterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenters", x => x.Id);
                    table.ForeignKey("FK_CostCenters_CostCenters_ParentCostCenterId", x => x.ParentCostCenterId, "CostCenters", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_CostCenters_FinanceBranches_BranchId", x => x.BranchId, "FinanceBranches", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinanceAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LedgerAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    AccountNumberMasked = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    RequiresReconciliation = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceAccounts", x => x.Id);
                    table.ForeignKey("FK_FinanceAccounts_ChartOfAccounts_LedgerAccountId", x => x.LedgerAccountId, "ChartOfAccounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_FinanceAccounts_FinanceBranches_BranchId", x => x.BranchId, "FinanceBranches", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JournalNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    AccountingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SourceDocumentType = table.Column<int>(type: "int", nullable: false),
                    SourceDocumentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PostedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReversalOfJournalEntryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                    table.ForeignKey("FK_JournalEntries_FinanceBranches_BranchId", x => x.BranchId, "FinanceBranches", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_JournalEntries_JournalEntries_ReversalOfJournalEntryId", x => x.ReversalOfJournalEntryId, "JournalEntries", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringFinancialObligations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FinanceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cycle = table.Column<int>(type: "int", nullable: false),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AutoPay = table.Column<bool>(type: "bit", nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringFinancialObligations", x => x.Id);
                    table.ForeignKey("FK_RecurringFinancialObligations_FinanceAccounts_FinanceAccountId", x => x.FinanceAccountId, "FinanceAccounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_RecurringFinancialObligations_FinanceBranches_BranchId", x => x.BranchId, "FinanceBranches", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryLines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JournalEntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LedgerAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CostCenterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    ExchangeRateToBase = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLines", x => x.Id);
                    table.ForeignKey("FK_JournalEntryLines_ChartOfAccounts_LedgerAccountId", x => x.LedgerAccountId, "ChartOfAccounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_JournalEntryLines_CostCenters_CostCenterId", x => x.CostCenterId, "CostCenters", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_JournalEntryLines_FinanceBranches_BranchId", x => x.BranchId, "FinanceBranches", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_JournalEntryLines_JournalEntries_JournalEntryId", x => x.JournalEntryId, "JournalEntries", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollRuns",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PeriodCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    ScheduledPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JournalEntryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollRuns", x => x.Id);
                    table.ForeignKey("FK_PayrollRuns_FinanceBranches_BranchId", x => x.BranchId, "FinanceBranches", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_PayrollRuns_JournalEntries_JournalEntryId", x => x.JournalEntryId, "JournalEntries", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SourceDocumentType = table.Column<int>(type: "int", nullable: false),
                    SourceDocumentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    FinanceAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CostCenterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    JournalEntryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    CounterpartyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CounterpartyName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAutomated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialTransactions", x => x.Id);
                    table.ForeignKey("FK_FinancialTransactions_CostCenters_CostCenterId", x => x.CostCenterId, "CostCenters", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_FinancialTransactions_FinanceAccounts_FinanceAccountId", x => x.FinanceAccountId, "FinanceAccounts", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_FinancialTransactions_FinanceBranches_BranchId", x => x.BranchId, "FinanceBranches", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_FinancialTransactions_JournalEntries_JournalEntryId", x => x.JournalEntryId, "JournalEntries", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollLines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PayrollRunId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxWithheld = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceWithheld = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollLines", x => x.Id);
                    table.ForeignKey("FK_PayrollLines_FinanceBranches_BranchId", x => x.BranchId, "FinanceBranches", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_PayrollLines_PayrollRuns_PayrollRunId", x => x.PayrollRunId, "PayrollRuns", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialApprovalLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ApproverUserId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmountThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_FinancialApprovalLogs", x => x.Id));

            migrationBuilder.CreateTable(
                name: "FinancialAuditLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BeforeJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AfterJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_FinancialAuditLogs", x => x.Id));

            migrationBuilder.CreateIndex("IX_AccountingPeriods_TenantId_StartsOn_EndsOn", "AccountingPeriods", new[] { "TenantId", "StartsOn", "EndsOn" }, unique: true);
            migrationBuilder.CreateIndex("IX_ChartOfAccounts_ParentAccountId", "ChartOfAccounts", "ParentAccountId");
            migrationBuilder.CreateIndex("IX_ChartOfAccounts_TenantId_Code", "ChartOfAccounts", new[] { "TenantId", "Code" }, unique: true);
            migrationBuilder.CreateIndex("IX_CostCenters_BranchId", "CostCenters", "BranchId");
            migrationBuilder.CreateIndex("IX_CostCenters_ParentCostCenterId", "CostCenters", "ParentCostCenterId");
            migrationBuilder.CreateIndex("IX_CostCenters_TenantId_Code", "CostCenters", new[] { "TenantId", "Code" }, unique: true);
            migrationBuilder.CreateIndex("IX_FinanceAccounts_BranchId", "FinanceAccounts", "BranchId");
            migrationBuilder.CreateIndex("IX_FinanceAccounts_LedgerAccountId", "FinanceAccounts", "LedgerAccountId");
            migrationBuilder.CreateIndex("IX_FinanceBranches_TenantId", "FinanceBranches", "TenantId");
            migrationBuilder.CreateIndex("IX_FinanceBranches_TenantId_Code", "FinanceBranches", new[] { "TenantId", "Code" }, unique: true);
            migrationBuilder.CreateIndex("IX_FinanceTenants_Code", "FinanceTenants", "Code", unique: true);
            migrationBuilder.CreateIndex("IX_FinancialApprovalLogs_TenantId_EntityName_EntityId", "FinancialApprovalLogs", new[] { "TenantId", "EntityName", "EntityId" });
            migrationBuilder.CreateIndex("IX_FinancialAuditLogs_TenantId_EntityName_EntityId_CreatedTime", "FinancialAuditLogs", new[] { "TenantId", "EntityName", "EntityId", "CreatedTime" });
            migrationBuilder.CreateIndex("IX_FinancialTransactions_BranchId", "FinancialTransactions", "BranchId");
            migrationBuilder.CreateIndex("IX_FinancialTransactions_CostCenterId", "FinancialTransactions", "CostCenterId");
            migrationBuilder.CreateIndex("IX_FinancialTransactions_FinanceAccountId", "FinancialTransactions", "FinanceAccountId");
            migrationBuilder.CreateIndex("IX_FinancialTransactions_JournalEntryId", "FinancialTransactions", "JournalEntryId");
            migrationBuilder.CreateIndex("IX_FinancialTransactions_TenantId_Code", "FinancialTransactions", new[] { "TenantId", "Code" }, unique: true);
            migrationBuilder.CreateIndex("IX_JournalEntries_BranchId", "JournalEntries", "BranchId");
            migrationBuilder.CreateIndex("IX_JournalEntries_ReversalOfJournalEntryId", "JournalEntries", "ReversalOfJournalEntryId");
            migrationBuilder.CreateIndex("IX_JournalEntries_TenantId_JournalNumber", "JournalEntries", new[] { "TenantId", "JournalNumber" }, unique: true);
            migrationBuilder.CreateIndex("IX_JournalEntryLines_BranchId", "JournalEntryLines", "BranchId");
            migrationBuilder.CreateIndex("IX_JournalEntryLines_CostCenterId", "JournalEntryLines", "CostCenterId");
            migrationBuilder.CreateIndex("IX_JournalEntryLines_JournalEntryId", "JournalEntryLines", "JournalEntryId");
            migrationBuilder.CreateIndex("IX_JournalEntryLines_LedgerAccountId", "JournalEntryLines", "LedgerAccountId");
            migrationBuilder.CreateIndex("IX_PayrollLines_BranchId", "PayrollLines", "BranchId");
            migrationBuilder.CreateIndex("IX_PayrollLines_PayrollRunId", "PayrollLines", "PayrollRunId");
            migrationBuilder.CreateIndex("IX_PayrollRuns_BranchId", "PayrollRuns", "BranchId");
            migrationBuilder.CreateIndex("IX_PayrollRuns_JournalEntryId", "PayrollRuns", "JournalEntryId");
            migrationBuilder.CreateIndex("IX_PayrollRuns_TenantId_PeriodCode_BranchId", "PayrollRuns", new[] { "TenantId", "PeriodCode", "BranchId" });
            migrationBuilder.CreateIndex("IX_RecurringFinancialObligations_BranchId", "RecurringFinancialObligations", "BranchId");
            migrationBuilder.CreateIndex("IX_RecurringFinancialObligations_FinanceAccountId", "RecurringFinancialObligations", "FinanceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FinancialApprovalLogs");
            migrationBuilder.DropTable(name: "FinancialAuditLogs");
            migrationBuilder.DropTable(name: "FinancialTransactions");
            migrationBuilder.DropTable(name: "JournalEntryLines");
            migrationBuilder.DropTable(name: "PayrollLines");
            migrationBuilder.DropTable(name: "RecurringFinancialObligations");
            migrationBuilder.DropTable(name: "CostCenters");
            migrationBuilder.DropTable(name: "PayrollRuns");
            migrationBuilder.DropTable(name: "FinanceAccounts");
            migrationBuilder.DropTable(name: "JournalEntries");
            migrationBuilder.DropTable(name: "ChartOfAccounts");
            migrationBuilder.DropTable(name: "AccountingPeriods");
            migrationBuilder.DropTable(name: "FinanceBranches");
            migrationBuilder.DropTable(name: "FinanceTenants");
        }
    }
}
