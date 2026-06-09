using CareNota.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CareNota.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options)
        : base(Options) { }

    // ── DbSets ────────────────────────────────────────────────────────────────
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Receptionist> Receptionists { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<AudioRecord> AudioRecords { get; set; }
    
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedication> PrescriptionMedications { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<LabTest> LabTests { get; set; }
    
    public DbSet<AISummary> AISummaries { get; set; }
    public DbSet<Diagnosis> Diagnoses { get; set; }
    
    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnModelCreating(ModelBuilder Builder)
    {
        base.OnModelCreating(Builder);

        // ── ApplicationUser → Admin (1-to-1) ─────────────────────────────────
        Builder.Entity<Admin>()
    .HasOne(A => A.User)
    .WithOne(U => U.Admin)
    .HasForeignKey<Admin>(A => A.UserId)
    .OnDelete(DeleteBehavior.Cascade);

        // ── ApplicationUser → Doctor (1-to-1) ─────────────────────────────────
        Builder.Entity<Doctor>()
            .HasOne(D => D.User)
            .WithOne(U => U.Doctor)
            .HasForeignKey<Doctor>(D => D.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── ApplicationUser → Patient (1-to-1) ────────────────────────────────
        Builder.Entity<Patient>()
            .HasOne(P => P.User)
            .WithOne(U => U.Patient)
            .HasForeignKey<Patient>(P => P.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── ApplicationUser → Receptionist (1-to-1) ───────────────────────────
        Builder.Entity<Receptionist>()
            .HasOne(R => R.User)
            .WithOne(U => U.Receptionist)
            .HasForeignKey<Receptionist>(R => R.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // ── Patient → Appointment (1-to-M) ────────────────────────────────────
        Builder.Entity<Appointment>()
            .HasOne(A => A.Patient)
            .WithMany(P => P.Appointments)
            .HasForeignKey(A => A.PatientID)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Receptionist → Appointment (1-to-M) ───────────────────────────────
        Builder.Entity<Appointment>()
            .HasOne(A => A.Receptionist)
            .WithMany(R => R.Appointments)
            .HasForeignKey(A => A.ReceptionistID)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Appointment → Visit (1-to-1) ───────────────────────────────────────
        Builder.Entity<Visit>()
            .HasOne(V => V.Appointment)
            .WithOne(A => A.Visit)
            .HasForeignKey<Visit>(V => V.AppointmentID)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Visit → Prescription (1-to-1) ─────────────────────────────────────
        Builder.Entity<Prescription>()
            .HasOne(P => P.Visit)
            .WithOne(V => V.Prescription)
            .HasForeignKey<Prescription>(P => P.VisitID)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Visit → LabTest (1-to-M) ───────────────────────────────────────────
        Builder.Entity<LabTest>()
            .HasOne(L => L.Visit)
            .WithMany(V => V.LabTests)
            .HasForeignKey(L => L.VisitID)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Visit → AudioRecord (1-to-1) ───────────────────────────────────────
        Builder.Entity<AudioRecord>()
            .HasOne(A => A.Visit)
            .WithOne(V => V.AudioRecord)
            .HasForeignKey<AudioRecord>(A => A.VisitID)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Visit → AISummary (1-to-M) ────────────────────────────────────────
        Builder.Entity<AISummary>()
            .HasOne(S => S.Visit)
            .WithMany(V => V.AISummaries)
            .HasForeignKey(S => S.VisitID)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Prescription → PrescriptionMedication (1-to-M) ────────────────────
        // ── Medication    → PrescriptionMedication (1-to-M) ────────────────────
        // Composite PK: (PrescriptionID, MedicationID)
        Builder.Entity<PrescriptionMedication>()
            .HasKey(PM => new { PM.PrescriptionID, PM.MedicationID });

        Builder.Entity<PrescriptionMedication>()
            .HasOne(PM => PM.Prescription)
            .WithMany(P => P.PrescriptionMedications)
            .HasForeignKey(PM => PM.PrescriptionID)
            .OnDelete(DeleteBehavior.Cascade);

        Builder.Entity<PrescriptionMedication>()
            .HasOne(PM => PM.Medication)
            .WithMany(M => M.PrescriptionMedications)
            .HasForeignKey(PM => PM.MedicationID)
            .OnDelete(DeleteBehavior.Restrict);


        // ADD — Diagnosis → Visit (simple one-to-many)
        Builder.Entity<Diagnosis>()
            .HasOne(D => D.Visit)
            .WithMany(V => V.Diagnoses)
            .HasForeignKey(D => D.VisitID)
            .OnDelete(DeleteBehavior.Cascade);


        // ── Reminder → Patient (M-to-1) ────────────────────────────────────────
        Builder.Entity<Reminder>()
            .HasOne(R => R.Patient)
            .WithMany(P => P.Reminders)
            .HasForeignKey(R => R.PatientID)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Reminder → Prescription (M-to-1) ──────────────────────────────────
        Builder.Entity<Reminder>()
            .HasOne(R => R.Prescription)
            .WithMany(P => P.Reminders)
            .HasForeignKey(R => R.PrescriptionID)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Reminder → Appointment (M-to-1) ───────────────────────────────────
        Builder.Entity<Reminder>()
            .HasOne(R => R.Appointment)
            .WithMany(A => A.Reminders)
            .HasForeignKey(R => R.AppointmentID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}