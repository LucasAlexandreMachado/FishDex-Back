# Fish Pokédex - Backend API Documentation

## 📋 Overview

**Project:** Fish Pokédex - REST API para gerenciar um registro de capturas de peixes  
**Technology Stack:** .NET 8, ASP.NET Core, Entity Framework Core, PostgreSQL  
**API Base URL:** `http://localhost:5000` (development)  
**HTTPS:** `https://localhost:5001`  
**Swagger/OpenAPI UI:** `http://localhost:5000/swagger`

---

## 🌐 CORS Configuration

✅ **CORS is fully enabled (wide-open policy)**
- **AllowAnyOrigin:** Allow requests from any domain
- **AllowAnyMethod:** Allow GET, POST, PUT, DELETE, OPTIONS, etc.
- **AllowAnyHeader:** Allow any headers
- **Frontend can consume this API from any origin without issues**

---

## 🗄️ Data Model

### 1. **Species** (Espécie de Peixe)
Entity representing a fish species.

```json
{
  "id": 1,
  "commonName": "Bass",
  "scientificName": "Micropterus salmoides",
  "catches": [
    // Array of Catch objects (loaded with Include)
  ]
}
```

**Fields:**
- `id` (int) - Primary Key, auto-generated
- `commonName` (string) - **Required** - Common name of the species
- `scientificName` (string) - Optional - Scientific classification
- `catches` (array) - Navigation property, loaded with `.Include()`

---

### 2. **Catch** (Captura)
Entity representing a fish catch event.

```json
{
  "id": 1,
  "location": "Lake Superior",
  "catchDate": "2026-04-09T23:37:17Z",
  "speciesId": 1,
  "species": {
    "id": 1,
    "commonName": "Bass",
    "scientificName": null,
    "catches": null
  },
  "catchDetail": {
    "id": 1,
    "weightKg": 2.5,
    "lengthCm": 45.5,
    "baitUsed": "Live minnow",
    "weatherCondition": "Sunny",
    "catchId": 1,
    "catch": null
  }
}
```

**Fields:**
- `id` (int) - Primary Key, auto-generated
- `location` (string) - **Required** - Location of the catch
- `catchDate` (datetime, UTC) - **Automatically set to UTC now on POST** - When the catch occurred
- `speciesId` (int) - **Required** - Foreign Key to Species
- `species` (object) - Navigation property to Species (loaded with `.Include()`)
- `catchDetail` (object) - Navigation property to CatchDetail (loaded with `.Include()`)

**Relationships:**
- **1:N with Species** - One Species has many Catches (DELETE RESTRICT - cannot delete species with linked catches)
- **1:1 with CatchDetail** - One Catch has one CatchDetail (CASCADE DELETE - deleting a Catch deletes its CatchDetail)

---

### 3. **CatchDetail** (Detalhes da Captura)
Entity with detailed information about a catch.

```json
{
  "id": 1,
  "weightKg": 2.5,
  "lengthCm": 45.5,
  "baitUsed": "Live minnow",
  "weatherCondition": "Sunny",
  "catchId": 1,
  "catch": {}
}
```

**Fields:**
- `id` (int) - Primary Key, auto-generated
- `weightKg` (decimal?) - Optional - Weight in kilograms
- `lengthCm` (decimal?) - Optional - Length in centimeters
- `baitUsed` (string?) - Optional - Type of bait used
- `weatherCondition` (string?) - Optional - Weather conditions during the catch
- `catchId` (int) - **Required** - Foreign Key to Catch (UNIQUE constraint)
- `catch` (object) - Navigation property to Catch

**Constraints:**
- **Unique Index on `catchId`** - Each Catch can have only ONE CatchDetail
- **CASCADE DELETE** - Deleting a Catch automatically deletes its CatchDetail

---

## 🔌 API Endpoints

### **SPECIES ENDPOINTS**

#### 1. GET `/api/species` - List all species
**Description:** Retrieve all fish species (lightweight, no related catches)

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "commonName": "Bass",
    "scientificName": "Micropterus salmoides",
    "catches": []
  },
  {
    "id": 2,
    "commonName": "Trout",
    "scientificName": "Salmo trutta",
    "catches": []
  }
]
```

---

#### 2. GET `/api/species/{id}` - Get single species with catches
**Description:** Retrieve a specific species including all its catches

**Parameters:** `id` (int) - Species ID

**Response:** `200 OK` / `404 Not Found`
```json
{
  "id": 1,
  "commonName": "Bass",
  "scientificName": "Micropterus salmoides",
  "catches": [
    {
      "id": 1,
      "location": "Lake Superior",
      "catchDate": "2026-04-09T23:37:17Z",
      "speciesId": 1,
      "species": null,
      "catchDetail": {}
    }
  ]
}
```

---

#### 3. POST `/api/species` - Create new species
**Description:** Create a new fish species

**Request Body:**
```json
{
  "commonName": "Bass",
  "scientificName": "Micropterus salmoides"
}
```

**Response:** `201 Created`
```json
{
  "id": 1,
  "commonName": "Bass",
  "scientificName": "Micropterus salmoides",
  "catches": []
}
```

**Validation:**
- `commonName` - **Required**
- `scientificName` - Optional

---

#### 4. PUT `/api/species/{id}` - Update species
**Description:** Update an existing species

**Parameters:** `id` (int) - Species ID

**Request Body:**
```json
{
  "id": 1,
  "commonName": "Largemouth Bass",
  "scientificName": "Micropterus salmoides"
}
```

**Response:** `204 No Content`

**Validation:**
- `id` in URL must match `id` in body

---

#### 5. DELETE `/api/species/{id}` - Delete species
**Description:** Delete a species

**Parameters:** `id` (int) - Species ID

**Response:** `204 No Content` / `404 Not Found` / `400 Bad Request`

**Error Case:**
```json
{
  "error": "Cannot delete species because there are linked catches."
}
```
Status: `400 Bad Request`

**Important:** Cannot delete a species if it has linked catches (DELETE RESTRICT enforced)

---

### **CATCHES ENDPOINTS**

#### 1. GET `/api/catches` - List all catches
**Description:** Retrieve all catches with species and detail information

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "location": "Lake Superior",
    "catchDate": "2026-04-09T23:37:17Z",
    "speciesId": 1,
    "species": {
      "id": 1,
      "commonName": "Bass",
      "scientificName": "Micropterus salmoides",
      "catches": null
    },
    "catchDetail": {
      "id": 1,
      "weightKg": 2.5,
      "lengthCm": 45.5,
      "baitUsed": "Live minnow",
      "weatherCondition": "Sunny",
      "catchId": 1,
      "catch": null
    }
  }
]
```

---

#### 2. GET `/api/catches/{id}` - Get single catch
**Description:** Retrieve a specific catch with species and detail

**Parameters:** `id` (int) - Catch ID

**Response:** `200 OK` / `404 Not Found`

---

#### 3. POST `/api/catches` - Create new catch
**Description:** Create a new catch record

**Request Body:**
```json
{
  "location": "Lake Superior",
  "speciesId": 1
}
```

**Important:** 
- `catchDate` is **automatically set to UTC now** - do NOT send it in the request
- You will receive it back with the timestamp when the catch was created

**Response:** `201 Created`
```json
{
  "id": 1,
  "location": "Lake Superior",
  "catchDate": "2026-04-09T23:37:17Z",
  "speciesId": 1,
  "species": null,
  "catchDetail": null
}
```

**Validation:**
- `location` - **Required**
- `speciesId` - **Required** (should reference an existing species)

---

#### 4. PUT `/api/catches/{id}` - Update catch
**Description:** Update a catch record

**Parameters:** `id` (int) - Catch ID

**Request Body:**
```json
{
  "id": 1,
  "location": "Lake Michigan",
  "catchDate": "2026-04-09T23:37:17Z",
  "speciesId": 1,
  "species": null,
  "catchDetail": null
}
```

**Response:** `204 No Content`

---

#### 5. DELETE `/api/catches/{id}` - Delete catch
**Description:** Delete a catch (its detail will be CASCADE deleted)

**Parameters:** `id` (int) - Catch ID

**Response:** `204 No Content` / `404 Not Found`

---

### **CATCHDETAILS ENDPOINTS**

#### 1. GET `/api/catchdetails` - List all catch details
**Description:** Retrieve all catch details with catch information

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "weightKg": 2.5,
    "lengthCm": 45.5,
    "baitUsed": "Live minnow",
    "weatherCondition": "Sunny",
    "catchId": 1,
    "catch": {
      "id": 1,
      "location": "Lake Superior",
      "catchDate": "2026-04-09T23:37:17Z",
      "speciesId": 1,
      "species": null,
      "catchDetail": null
    }
  }
]
```

---

#### 2. GET `/api/catchdetails/{id}` - Get single catch detail
**Description:** Retrieve a specific catch detail

**Parameters:** `id` (int) - CatchDetail ID

**Response:** `200 OK` / `404 Not Found`

---

#### 3. GET `/api/catchdetails/catch/{catchId}` - Get detail by catch ID
**Description:** Retrieve catch detail by the catch ID (custom route)

**Parameters:** `catchId` (int) - Catch ID

**Response:** `200 OK` / `404 Not Found`

**Use Case:** Given a Catch ID, get its associated CatchDetail

---

#### 4. POST `/api/catchdetails` - Create catch detail
**Description:** Add detailed information to a catch

**Request Body:**
```json
{
  "weightKg": 2.5,
  "lengthCm": 45.5,
  "baitUsed": "Live minnow",
  "weatherCondition": "Sunny",
  "catchId": 1
}
```

**Response:** `201 Created`

**Validation & Error Cases:**

1. **Catch doesn't exist:** `400 Bad Request`
```json
{
  "error": "The referenced Catch does not exist."
}
```

2. **Catch already has detail:** `409 Conflict`
```json
{
  "error": "This Catch already has a detail registered."
}
```

**Important:** Each Catch can have **only ONE** associated CatchDetail

---

#### 5. PUT `/api/catchdetails/{id}` - Update catch detail
**Description:** Update a catch detail

**Parameters:** `id` (int) - CatchDetail ID

**Request Body:**
```json
{
  "id": 1,
  "weightKg": 3.0,
  "lengthCm": 48.0,
  "baitUsed": "Artificial lure",
  "weatherCondition": "Rainy",
  "catchId": 1,
  "catch": null
}
```

**Response:** `204 No Content`

---

#### 6. DELETE `/api/catchdetails/{id}` - Delete catch detail
**Description:** Delete a catch detail

**Parameters:** `id` (int) - CatchDetail ID

**Response:** `204 No Content` / `404 Not Found`

---

## 📊 HTTP Status Codes Reference

| Code | Meaning | Common Use |
|------|---------|-----------|
| 200 | OK | GET, PUT success |
| 201 | Created | POST success |
| 204 | No Content | PUT, DELETE success |
| 400 | Bad Request | Invalid input, validation error, constraint violation |
| 404 | Not Found | Resource not found |
| 409 | Conflict | Business logic conflict (e.g., catch already has detail) |
| 500 | Internal Server Error | Server error |

---

## 🚀 Server Configuration

**Development Environment:**
- **Port (HTTP):** 5000
- **Port (HTTPS):** 5001
- **HTTPS Certificate:** Self-signed (not trusted - warning in logs is normal)

**Database:**
- **Type:** PostgreSQL 16
- **Host:** localhost
- **Port:** 5432
- **Database:** fish_pokedex
- **Username:** postgres
- **Password:** postgres123
- **Connection String:** `Host=localhost;Port=5432;Database=fish_pokedex;Username=postgres;Password=postgres123`

**Docker Compose Services:**
- **postgresql:** PostgreSQL 16 database
- **pgAdmin:** Web UI for database management (port 8080, admin@admin.com / admin123)

---

## 💡 Frontend Integration Tips

### 1. **Base URL Configuration**
Set your frontend API client base URL to `http://localhost:5000/api`

### 2. **Automatic CatchDate**
Don't send `catchDate` when creating a Catch:
```javascript
// ❌ Wrong - CatchDate will be ignored
POST /api/catches {
  location: "Lake",
  catchDate: "2026-04-09T23:37:17Z",
  speciesId: 1
}

// ✅ Correct
POST /api/catches {
  location: "Lake",
  speciesId: 1
}
```

### 3. **Cascade Delete Behavior**
When you DELETE a Catch, its CatchDetail is automatically deleted:
```javascript
DELETE /api/catches/1
// This also deletes any CatchDetail with catchId=1
```

### 4. **Delete Restriction on Species**
Cannot delete a Species with linked catches - handle the 400 error:
```javascript
DELETE /api/species/1
// Returns 400 if species has any catches
// Response: "Cannot delete species because there are linked catches."
```

### 5. **Unique CatchDetail per Catch**
Each Catch can have exactly one CatchDetail:
```javascript
// After creating Catch Detail for catchId=1
POST /api/catchdetails { catchId: 1, ... } // ✅ Success
POST /api/catchdetails { catchId: 1, ... } // ❌ 409 Conflict - already has detail
```

### 6. **Get CatchDetail by Catch**
Use the custom route to get a catch's detail by Catch ID (not CatchDetail ID):
```javascript
GET /api/catchdetails/catch/1
// Returns the CatchDetail associated with Catch ID 1
// Returns 404 if the catch has no detail registered yet
```

### 7. **Eager Loading**
All related entities are automatically included:
```javascript
GET /api/catches/1
// Returns: { id, location, catchDate, speciesId, species: {...}, catchDetail: {...} }
```

### 8. **JSON Serialization**
API handles circular reference prevention automatically (ReferenceHandler.IgnoreCycles)

---

## ⚙️ Example Request/Response Flows

### **Create a Complete Catch with Details**

**Step 1: Create a Species**
```http
POST /api/species
Content-Type: application/json

{
  "commonName": "Bass",
  "scientificName": "Micropterus salmoides"
}
```
Response (201):
```json
{
  "id": 1,
  "commonName": "Bass",
  "scientificName": "Micropterus salmoides",
  "catches": []
}
```

**Step 2: Create a Catch**
```http
POST /api/catches
Content-Type: application/json

{
  "location": "Lake Superior",
  "speciesId": 1
}
```
Response (201):
```json
{
  "id": 1,
  "location": "Lake Superior",
  "catchDate": "2026-04-09T23:37:17.123Z",
  "speciesId": 1,
  "species": null,
  "catchDetail": null
}
```

**Step 3: Add Catch Details**
```http
POST /api/catchdetails
Content-Type: application/json

{
  "weightKg": 2.5,
  "lengthCm": 45.5,
  "baitUsed": "Live minnow",
  "weatherCondition": "Sunny",
  "catchId": 1
}
```
Response (201):
```json
{
  "id": 1,
  "weightKg": 2.5,
  "lengthCm": 45.5,
  "baitUsed": "Live minnow",
  "weatherCondition": "Sunny",
  "catchId": 1,
  "catch": null
}
```

**Step 4: Retrieve Complete Catch**
```http
GET /api/catches/1
```
Response (200):
```json
{
  "id": 1,
  "location": "Lake Superior",
  "catchDate": "2026-04-09T23:37:17.123Z",
  "speciesId": 1,
  "species": {
    "id": 1,
    "commonName": "Bass",
    "scientificName": "Micropterus salmoides",
    "catches": null
  },
  "catchDetail": {
    "id": 1,
    "weightKg": 2.5,
    "lengthCm": 45.5,
    "baitUsed": "Live minnow",
    "weatherCondition": "Sunny",
    "catchId": 1,
    "catch": null
  }
}
```

---

## 🔗 Related Documentation

- [README.md](./README.md) - Project overview
- [context-backend.md](./context/context-backend.md) - Additional backend architecture details
- [SETUP.md](./SETUP.md) - Development setup instructions

---

## 📝 Notes for Frontend Developer

- ✅ API is fully operational and tested
- ✅ CORS is enabled - no origin restrictions
- ✅ All endpoints follow REST conventions
- ✅ Consistent error handling with appropriate HTTP status codes
- ✅ Automatic timezone handling (UTC)
- ✅ Eager loading prevents N+1 query problems
- ✅ Swagger/OpenAPI documentation available at `/swagger`

---

**Backend Status:** Complete ✅  
**Ready for Frontend Integration:** Yes ✅  
**Last Updated:** April 9, 2026
