# EasyClinic: Deferred Items

## Clean Architecture phase
- [ ] Replace FakeUserStore with ASP.NET Core Identity
- [ ] Refresh Token
- [ ] Data Protection
- [ ] Repository / Unit of Work (or IAppDbContext abstraction)
- [ ] Cascade -> Restrict on remaining FKs (Doctor->Specialization, DoctorService->*)
- [ ] Soft Delete for patients and medical records
- [ ] Clinic time zone for working-hours check
- [ ] Validators for PATCH DTOs

## Course modules skipped (return after Clean Architecture)
- [ ] Module 21: Performance (Rate Limiting on /login first, then caching)
- [ ] Module 22: Docker
- [ ] Module 23: Observability
- [ ] Module 24: Background Services
- [ ] Module 14: API Versioning

## Optional
- [ ] Swagger OperationFilter for 401/403
- [ ] Scalar