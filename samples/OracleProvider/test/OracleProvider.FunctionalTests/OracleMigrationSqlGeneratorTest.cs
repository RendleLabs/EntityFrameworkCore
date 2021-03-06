// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore
{
    public class OracleMigrationSqlGeneratorTest : MigrationSqlGeneratorTestBase
    {
        [Fact]
        public virtual void AddColumnOperation_with_computedSql()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "FullName",
                    ClrType = typeof(string),
                    ComputedColumnSql = "FirstName || ' ' || LastName"
                });

            Assert.Equal(
                "ALTER TABLE \"People\" ADD \"FullName\" AS (FirstName || ' ' || LastName)",
                Sql);
        }

        public override void AddColumnOperation_with_computed_column_SQL()
        {
            base.AddColumnOperation_with_computed_column_SQL();

            Assert.Equal(
                "ALTER TABLE \"People\" ADD \"Birthday\" AS (CURRENT_TIMESTAMP)",
                Sql);
        }

        [Fact]
        public virtual void AddColumnOperation_identity()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "People",
                    Name = "Id",
                    ClrType = typeof(int),
                    ColumnType = "int",
                    DefaultValue = 0,
                    IsNullable = false,
                    [OracleAnnotationNames.ValueGenerationStrategy] =
                    OracleValueGenerationStrategy.IdentityColumn
                });

            Assert.Equal(
                "ALTER TABLE \"People\" ADD \"Id\" int DEFAULT 0 GENERATED BY DEFAULT ON NULL AS IDENTITY",
                Sql);
        }

        public override void AddColumnOperation_without_column_type()
        {
            base.AddColumnOperation_without_column_type();

            Assert.Equal(
                "ALTER TABLE \"People\" ADD \"Alias\" NVARCHAR2(2000) NOT NULL",
                Sql);
        }

        public override void AddColumnOperation_with_unicode_no_model()
        {
            base.AddColumnOperation_with_unicode_no_model();

            Assert.Equal(
                "ALTER TABLE \"Person\" ADD \"Name\" VARCHAR2(4000) NULL",
                Sql);
        }

        public override void AddColumnOperation_with_maxLength()
        {
            base.AddColumnOperation_with_maxLength();

            Assert.Equal(
                "ALTER TABLE \"Person\" ADD \"Name\" NVARCHAR2(30) NULL",
                Sql);
        }

        public override void AddColumnOperation_with_maxLength_overridden()
        {
            base.AddColumnOperation_with_maxLength_overridden();

            Assert.Equal(
                "ALTER TABLE \"Person\" ADD \"Name\" NVARCHAR2(32) NULL",
                Sql);
        }

        public override void AddColumnOperation_with_maxLength_on_derived()
        {
            base.AddColumnOperation_with_maxLength_on_derived();

            Assert.Equal(
                "ALTER TABLE \"Person\" ADD \"Name\" NVARCHAR2(30) NULL",
                Sql);
        }

        public override void AddColumnOperation_with_ansi()
        {
            base.AddColumnOperation_with_ansi();

            Assert.Equal(
                "ALTER TABLE \"Person\" ADD \"Name\" VARCHAR2(4000) NULL",
                Sql);
        }

        public override void AddColumnOperation_with_unicode_overridden()
        {
            base.AddColumnOperation_with_unicode_overridden();

            Assert.Equal(
                "ALTER TABLE \"Person\" ADD \"Name\" NVARCHAR2(2000) NULL",
                Sql);
        }

        public override void AddColumnOperation_with_shared_column()
        {
            base.AddColumnOperation_with_shared_column();

            Assert.Equal(
                "ALTER TABLE \"Base\" ADD \"Foo\" NVARCHAR2(2000) NULL",
                Sql);
        }

        [Fact]
        public virtual void AddColumnOperation_with_rowversion_overridden()
        {
            Generate(
                modelBuilder => modelBuilder.Entity("Person").Property<byte[]>("RowVersion"),
                new AddColumnOperation
                {
                    Table = "Person",
                    Name = "RowVersion",
                    ClrType = typeof(byte[]),
                    IsRowVersion = true,
                    IsNullable = true
                });

            Assert.Equal(
                "ALTER TABLE \"Person\" ADD \"RowVersion\" RAW(8) NULL",
                Sql);
        }

        [Fact]
        public virtual void AddColumnOperation_with_rowversion_no_model()
        {
            Generate(
                new AddColumnOperation
                {
                    Table = "Person",
                    Name = "RowVersion",
                    ClrType = typeof(byte[]),
                    IsRowVersion = true,
                    IsNullable = true
                });

            Assert.Equal(
                "ALTER TABLE \"Person\" ADD \"RowVersion\" RAW(8) NULL",
                Sql);
        }

        [Fact]
        public virtual void AddPrimaryKeyOperation()
        {
            Generate(
                new AddPrimaryKeyOperation
                {
                    Table = "People",
                    Columns = new[] { "Id" }
                });

            Assert.Equal(
                "ALTER TABLE \"People\" ADD PRIMARY KEY (\"Id\")",
                Sql);
        }

        [Fact(Skip = "TODO")]
        public override void AlterColumnOperation()
        {
            base.AlterColumnOperation();

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'dbo.People') AND \"c\".\"name\" = N'LuckyNumber');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"dbo\".\"People\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"dbo\".\"People\" ALTER COLUMN \"LuckyNumber\" int NOT NULL;" + EOL +
                "ALTER TABLE \"dbo\".\"People\" ADD DEFAULT 7 FOR \"LuckyNumber\";" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public override void AlterColumnOperation_without_column_type()
        {
            base.AlterColumnOperation_without_column_type();

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'People') AND \"c\".\"name\" = N'LuckyNumber');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"People\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"People\" ALTER COLUMN \"LuckyNumber\" int NOT NULL;" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_with_identity()
        {
            Generate(
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "Id",
                    ClrType = typeof(int),
                    [OracleAnnotationNames.ValueGenerationStrategy] =
                    OracleValueGenerationStrategy.IdentityColumn
                });

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'People') AND \"c\".\"name\" = N'Id');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"People\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"People\" ALTER COLUMN \"Id\" int NOT NULL;" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_computed()
        {
            Generate(
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "FullName",
                    ClrType = typeof(string),
                    ComputedColumnSql = "\"FirstName\" + ' ' + \"LastName\""
                });

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'People') AND \"c\".\"name\" = N'FullName');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"People\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"People\" DROP COLUMN \"FullName\";" + EOL +
                "ALTER TABLE \"People\" ADD \"FullName\" AS \"FirstName\" + ' ' + \"LastName\";" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_computed_with_index()
        {
            Generate(
                modelBuilder => modelBuilder
                    .HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.1.0")
                    .Entity(
                        "Person", x =>
                            {
                                x.Property<string>("FullName").HasComputedColumnSql("\"FirstName\" + ' ' + \"LastName\"");
                                x.HasIndex("FullName");
                            }),
                new AlterColumnOperation
                {
                    Table = "Person",
                    Name = "FullName",
                    ClrType = typeof(string),
                    ComputedColumnSql = "\"FirstName\" + ' ' + \"LastName\"",
                    OldColumn = new ColumnOperation
                    {
                        ClrType = typeof(string),
                        ComputedColumnSql = "\"LastName\" + ', ' + \"FirstName\""
                    }
                });

            Assert.Equal(
                "DROP INDEX \"IX_Person_FullName\" ON \"Person\";" + EOL +
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'Person') AND \"c\".\"name\" = N'FullName');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"Person\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"Person\" DROP COLUMN \"FullName\";" + EOL +
                "ALTER TABLE \"Person\" ADD \"FullName\" AS \"FirstName\" + ' ' + \"LastName\";" + EOL +
                "CREATE INDEX \"IX_Person_FullName\" ON \"Person\" (\"FullName\");" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_with_index_no_narrowing()
        {
            Generate(
                modelBuilder => modelBuilder
                    .HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.1.0")
                    .Entity(
                        "Person", x =>
                            {
                                x.Property<string>("Name");
                                x.HasIndex("Name");
                            }),
                new AlterColumnOperation
                {
                    Table = "Person",
                    Name = "Name",
                    ClrType = typeof(string),
                    IsNullable = true,
                    OldColumn = new ColumnOperation
                    {
                        ClrType = typeof(string),
                        IsNullable = false
                    }
                });

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'Person') AND \"c\".\"name\" = N'Name');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"Person\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"Person\" ALTER COLUMN \"Name\" NVARCHAR2(450) NULL;" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_with_index()
        {
            Generate(
                modelBuilder => modelBuilder
                    .HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.1.0")
                    .Entity(
                        "Person", x =>
                            {
                                x.Property<string>("Name").HasMaxLength(30);
                                x.HasIndex("Name");
                            }),
                new AlterColumnOperation
                {
                    Table = "Person",
                    Name = "Name",
                    ClrType = typeof(string),
                    MaxLength = 30,
                    IsNullable = true,
                    OldColumn = new ColumnOperation
                    {
                        ClrType = typeof(string),
                        IsNullable = true
                    }
                });

            Assert.Equal(
                "DROP INDEX \"IX_Person_Name\" ON \"Person\";" + EOL +
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'Person') AND \"c\".\"name\" = N'Name');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"Person\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"Person\" ALTER COLUMN \"Name\" NVARCHAR2(30) NULL;" + EOL +
                "CREATE INDEX \"IX_Person_Name\" ON \"Person\" (\"Name\");" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_with_index_no_oldColumn()
        {
            Generate(
                modelBuilder => modelBuilder
                    .HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.0.0-rtm")
                    .Entity(
                        "Person", x =>
                            {
                                x.Property<string>("Name").HasMaxLength(30);
                                x.HasIndex("Name");
                            }),
                new AlterColumnOperation
                {
                    Table = "Person",
                    Name = "Name",
                    ClrType = typeof(string),
                    MaxLength = 30,
                    IsNullable = true,
                    OldColumn = new ColumnOperation()
                });

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'Person') AND \"c\".\"name\" = N'Name');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"Person\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"Person\" ALTER COLUMN \"Name\" NVARCHAR2(30) NULL;" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_with_composite_index()
        {
            Generate(
                modelBuilder => modelBuilder
                    .HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.1.0")
                    .Entity(
                        "Person", x =>
                            {
                                x.Property<string>("FirstName").IsRequired();
                                x.Property<string>("LastName");
                                x.HasIndex("FirstName", "LastName");
                            }),
                new AlterColumnOperation
                {
                    Table = "Person",
                    Name = "FirstName",
                    ClrType = typeof(string),
                    IsNullable = false,
                    OldColumn = new ColumnOperation
                    {
                        ClrType = typeof(string),
                        IsNullable = true
                    }
                });

            Assert.Equal(
                "DROP INDEX \"IX_Person_FirstName_LastName\" ON \"Person\";" + EOL +
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'Person') AND \"c\".\"name\" = N'FirstName');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"Person\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"Person\" ALTER COLUMN \"FirstName\" NVARCHAR2(450) NOT NULL;" + EOL +
                "CREATE INDEX \"IX_Person_FirstName_LastName\" ON \"Person\" (\"FirstName\", \"LastName\");" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_with_added_index()
        {
            Generate(
                modelBuilder => modelBuilder
                    .HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.1.0")
                    .Entity(
                        "Person", x =>
                            {
                                x.Property<string>("Name").HasMaxLength(30);
                                x.HasIndex("Name");
                            }),
                new AlterColumnOperation
                {
                    Table = "Person",
                    Name = "Name",
                    ClrType = typeof(string),
                    MaxLength = 30,
                    IsNullable = true,
                    OldColumn = new ColumnOperation
                    {
                        ClrType = typeof(string),
                        IsNullable = true
                    }
                },
                new CreateIndexOperation
                {
                    Name = "IX_Person_Name",
                    Table = "Person",
                    Columns = new[] { "Name" }
                });

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'Person') AND \"c\".\"name\" = N'Name');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"Person\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"Person\" ALTER COLUMN \"Name\" NVARCHAR2(30) NULL;" + EOL +
                "GO" + EOL +
                EOL +
                "CREATE INDEX \"IX_Person_Name\" ON \"Person\" (\"Name\");" + EOL,
                Sql);
        }

        [Fact(Skip = "TODO")]
        public virtual void AlterColumnOperation_identity()
        {
            Generate(
                modelBuilder => modelBuilder.HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.1.0"),
                new AlterColumnOperation
                {
                    Table = "Person",
                    Name = "Id",
                    ClrType = typeof(long),
                    [OracleAnnotationNames.ValueGenerationStrategy] = OracleValueGenerationStrategy.IdentityColumn,
                    OldColumn = new ColumnOperation
                    {
                        ClrType = typeof(int),
                        [OracleAnnotationNames.ValueGenerationStrategy] = OracleValueGenerationStrategy.IdentityColumn
                    }
                });

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'Person') AND \"c\".\"name\" = N'Id');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"Person\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"Person\" ALTER COLUMN \"Id\" bigint NOT NULL;" + EOL,
                Sql);
        }

        [Fact]
        public virtual void AlterColumnOperation_add_identity()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => Generate(
                    modelBuilder => modelBuilder.HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.1.0"),
                    new AlterColumnOperation
                    {
                        Table = "Person",
                        Name = "Id",
                        ClrType = typeof(int),
                        [OracleAnnotationNames.ValueGenerationStrategy] = OracleValueGenerationStrategy.IdentityColumn,
                        OldColumn = new ColumnOperation
                        {
                            ClrType = typeof(int)
                        }
                    }));

            Assert.Equal(OracleStrings.AlterIdentityColumn, ex.Message);
        }

        [Fact]
        public virtual void AlterColumnOperation_remove_identity()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => Generate(
                    modelBuilder => modelBuilder.HasAnnotation(CoreAnnotationNames.ProductVersionAnnotation, "1.1.0"),
                    new AlterColumnOperation
                    {
                        Table = "Person",
                        Name = "Id",
                        ClrType = typeof(int),
                        OldColumn = new ColumnOperation
                        {
                            ClrType = typeof(int),
                            [OracleAnnotationNames.ValueGenerationStrategy] = OracleValueGenerationStrategy.IdentityColumn
                        }
                    }));

            Assert.Equal(OracleStrings.AlterIdentityColumn, ex.Message);
        }

        [Fact]
        public virtual void CreateUserOperation()
        {
            Generate(new OracleCreateUserOperation { UserName = "Northwind" });

            Assert.Equal(
                @"BEGIN
                             EXECUTE IMMEDIATE 'CREATE USER Northwind IDENTIFIED BY Northwind';
                             EXECUTE IMMEDIATE 'GRANT DBA TO Northwind';
                           END;",
                Sql);
        }

        public override void CreateIndexOperation_nonunique()
        {
            base.CreateIndexOperation_nonunique();

            Assert.Equal(
                "CREATE INDEX \"IX_People_Name\" ON \"People\" (\"Name\")",
                Sql);
        }

        public override void CreateIndexOperation_unique()
        {
            base.CreateIndexOperation_unique();

            Assert.Equal(
                "CREATE UNIQUE INDEX \"IX_People_Name\" ON \"People\" (\"FirstName\", \"LastName\")",
                Sql);
        }

        [Fact]
        public virtual void CreateIndexOperation_unique_bound_not_null()
        {
            Generate(
                modelBuilder => modelBuilder.Entity("People").Property<string>("Name").IsRequired(),
                new CreateIndexOperation
                {
                    Name = "IX_People_Name",
                    Table = "People",
                    Columns = new[] { "Name" },
                    IsUnique = true
                });

            Assert.Equal(
                "CREATE UNIQUE INDEX \"IX_People_Name\" ON \"People\" (\"Name\")",
                Sql);
        }

        [Fact]
        public virtual void CreateSchemaOperation_dbo()
        {
            Generate(new EnsureSchemaOperation { Name = "dbo" });

            Assert.Equal(
                "",
                Sql);
        }

        [Fact(Skip = "TODO")]
        public override void DropColumnOperation()
        {
            base.DropColumnOperation();

            Assert.Equal(
                "DECLARE @var0 sysname;" + EOL +
                "SELECT @var0 = \"d\".\"name\"" + EOL +
                "FROM \"sys\".\"default_constraints\" \"d\"" + EOL +
                "INNER JOIN \"sys\".\"columns\" \"c\" ON \"d\".\"parent_column_id\" = \"c\".\"column_id\" AND \"d\".\"parent_object_id\" = \"c\".\"object_id\"" + EOL +
                "WHERE (\"d\".\"parent_object_id\" = OBJECT_ID(N'dbo.People') AND \"c\".\"name\" = N'LuckyNumber');" + EOL +
                "IF @var0 IS NOT NULL EXEC(N'ALTER TABLE \"dbo\".\"People\" DROP CONSTRAINT \"' + @var0 + '\";');" + EOL +
                "ALTER TABLE \"dbo\".\"People\" DROP COLUMN \"LuckyNumber\";" + EOL,
                Sql);
        }

        [Fact]
        public virtual void DropUserOperation()
        {
            Generate(new OracleDropUserOperation { UserName = "Northwind" });

            Assert.Equal(
                @"BEGIN
                         FOR v_cur IN (SELECT sid, serial# FROM v$session WHERE username = 'NORTHWIND') LOOP
                            EXECUTE IMMEDIATE ('ALTER SYSTEM KILL SESSION ''' || v_cur.sid || ',' || v_cur.serial# || ''' IMMEDIATE');
                         END LOOP;
                         EXECUTE IMMEDIATE 'DROP USER Northwind CASCADE';
                       END;",
                Sql);
        }

        public override void DropIndexOperation()
        {
            base.DropIndexOperation();

            Assert.Equal(
                "DROP INDEX \"IX_People_Name\" ON \"People\"",
                Sql);
        }

        [Fact]
        public virtual void RenameColumnOperation()
        {
            Generate(
                new RenameColumnOperation
                {
                    Table = "People",
                    Schema = "dbo",
                    Name = "Name",
                    NewName = "FullName"
                });

            Assert.Equal(
                "EXEC sp_rename N'dbo.People.Name', N'FullName', N'COLUMN'",
                Sql);
        }

        [Fact]
        public virtual void RenameIndexOperation()
        {
            Generate(
                new RenameIndexOperation
                {
                    Table = "People",
                    Schema = "dbo",
                    Name = "IX_People_Name",
                    NewName = "IX_People_FullName"
                });

            Assert.Equal(
                "EXEC sp_rename N'dbo.People.IX_People_Name', N'IX_People_FullName', N'INDEX'",
                Sql);
        }

        [Fact]
        public virtual void RenameIndexOperations_throws_when_no_table()
        {
            var migrationBuilder = new MigrationBuilder("Oracle");

            migrationBuilder.RenameIndex(
                name: "IX_OldIndex",
                newName: "IX_NewIndex");

            var ex = Assert.Throws<InvalidOperationException>(
                () => Generate(migrationBuilder.Operations.ToArray()));

            Assert.Equal(OracleStrings.IndexTableRequired, ex.Message);
        }

        [Fact]
        public virtual void RenameSequenceOperation()
        {
            Generate(
                new RenameSequenceOperation
                {
                    Name = "EntityFrameworkHiLoSequence",
                    Schema = "dbo",
                    NewName = "MySequence"
                });

            Assert.Equal(
                "EXEC sp_rename N'dbo.EntityFrameworkHiLoSequence', N'MySequence'",
                Sql);
        }

        [Fact]
        public virtual void RenameTableOperation()
        {
            Generate(
                new RenameTableOperation
                {
                    Name = "People",
                    Schema = "dbo",
                    NewName = "Person"
                });

            Assert.Equal(
                "EXEC sp_rename N'dbo.People', N'Person'",
                Sql);
        }

        [Fact]
        public virtual void SqlOperation_handles_backslash()
        {
            Generate(
                new SqlOperation
                {
                    Sql = @"-- Multiline \" + EOL +
                          "comment"
                });

            Assert.Equal(
                "-- Multiline comment",
                Sql);
        }

        [Fact(Skip = "TODO")]
        public override void InsertDataOperation()
        {
            base.InsertDataOperation();

            Assert.Equal(
                "IF EXISTS (SELECT * FROM \"sys\".\"identity_columns\" WHERE \"object_id\" = OBJECT_ID(N'People'))" + EOL +
                "    SET IDENTITY_INSERT \"People\" ON;" + EOL +
                "INSERT INTO \"People\" (\"Id\", \"Full Name\")" + EOL +
                "VALUES (0, NULL)," + EOL +
                "       (1, N'Daenerys Targaryen')," + EOL +
                "       (2, N'John Snow')," + EOL +
                "       (3, N'Arya Stark')," + EOL +
                "       (4, N'Harry Strickland');" + EOL +
                "IF EXISTS (SELECT * FROM \"sys\".\"identity_columns\" WHERE \"object_id\" = OBJECT_ID(N'People'))" + EOL +
                "    SET IDENTITY_INSERT \"People\" OFF;" + EOL,
                Sql);
        }

        public OracleMigrationSqlGeneratorTest()
            : base(OracleTestHelpers.Instance)
        {
        }
    }
}
