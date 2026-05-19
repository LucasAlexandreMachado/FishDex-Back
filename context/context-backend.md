# Project Context: Fish Pokédex (Backend API)

This document provides context for building a .NET 8 Web API for a Fish Catch Registry (Pokédex style).

## 🚀 MANDATORY ARCHITECTURE & BUILD RULES
1. **Framework:** .NET 8 + Entity Framework Core + Npgsql (PostgreSQL).
2. **Language:** All code, variables, and domain models MUST be in standard English.
3. **Relationships:** - **1:N:** One `Species` has many `Catches`. (Restrict Delete).
   - **1:1:** One `Catch` has one `CatchDetail`. (Cascade Delete).
4. **Build Fixes (Crucial):**
   - Use `ReferenceHandler.IgnoreCycles` in `Program.cs` to prevent JSON serialization loops.
   - Configure a wide-open CORS policy in `Program.cs`.
   - Explicitly define the 1:1 Foreign Key in `OnModelCreating` using `.HasForeignKey<CatchDetail>(d => d.CatchId)`.

## 🐳 DATABASE & DOCKER SETUP
The AI must generate a `docker-compose.yml` to run the database.
* **PostgreSQL Version:** `16-alpine`.
* **Database Name:** `fish_pokedex`.
* **User/Password:** `postgres` / `postgres123`.
* **Port:** `5432`.
* **pgAdmin:** Include a service for pgAdmin on port `8080` (admin@admin.com / admin123).
* **Connection String:** Match these Docker credentials in `appsettings.json` (Host=localhost for local dev).

## 🗄️ DATA MODEL

### 1. `Species` (Side 1 of 1:N)
* `int Id`
* `string CommonName` (Required)
* `string ScientificName` (Optional)
* `ICollection<Catch> Catches` (Initialized as empty list)

### 2. `Catch` (Side N of 1:N / Side 1 of 1:1)
* `int Id`
* `string Location` (Required)
* `DateTime CatchDate` (UTC)
* `int SpeciesId` (FK) -> `Species Species` (Navigation)
* `CatchDetail CatchDetail` (Navigation)

### 3. `CatchDetail` (Side 1 of 1:1)
* `int Id`
* `decimal? WeightKg`
* `decimal? LengthCm`
* `string BaitUsed`
* `string WeatherCondition`
* `int CatchId` (FK + UNIQUE Index) -> `Catch Catch` (Navigation)

## ⚙️ DB CONTEXT CONFIG (Fluent API)
Inside `AppDbContext.OnModelCreating`:
1. **1:N (`Species` -> `Catch`):** Set `.OnDelete(DeleteBehavior.Restrict)`.
2. **1:1 (`Catch` -> `CatchDetail`):** Set `.OnDelete(DeleteBehavior.Cascade)`.
3. **Index:** Ensure `.HasIndex(d => d.CatchId).IsUnique()` to enforce the 1:1 constraint.

## 🛠️ CONTROLLER LOGIC
* **REST Standards:** Return `CreatedAtAction` on POST, `NoContent` on PUT/DELETE.
* **Eager Loading:** ALWAYS use `.Include()` in GET methods to fetch related entities (except for the `SpeciesController` GET All, which should remain lightweight).
* **SpeciesController:** In the DELETE method, use a try/catch for `DbUpdateException`. If caught, return `400 Bad Request` with a friendly message stating it cannot be deleted because there are linked catches.
* **CatchesController:** In the POST method, automatically set `CatchDate = DateTime.UtcNow` before saving.
* **CatchDetailsController:**
  * **Custom Route:** `GET /api/catchdetails/catch/{catchId}` (Returns 404 if not found).
  * **POST Validation:** Check if the referenced `Catch` exists (return 400 if not) and if it *already has* a detail registered (return 409 Conflict if true).
  * PUT and DELETE operate by the `CatchDetail`'s own ID.

## 📋 STARTUP INSTRUCTIONS FOR AI
1. Create the `.NET 8 Web API` project (disable OpenAPI, use standard Swagger generation).
2. Add necessary NuGet packages: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`.
3. Create the `docker-compose.yml` and configure `appsettings.json`.
4. Implement Models, Data Context, and Controllers.
