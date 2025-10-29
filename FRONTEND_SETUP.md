# Frontend Setup Guide - Orange Car Rental

**Note:** The automated Nx setup had issues with interactive prompts. Follow these manual steps.

---

## Quick Setup (5-10 minutes)

### Step 1: Create Nx Workspace

Open a terminal in the `src` directory:

```bash
cd C:\Users\heiko\claude-orange-car-rental\src

# Create Nx workspace - when prompted:
# - AI agents: Press ENTER (skip all)
# - Or select "None" if available
npx create-nx-workspace@latest frontend
```

**When prompted, answer:**
- Preset: `angular-monorepo`
- Application name: `public-portal`
- Stylesheet: `css`
- Enable routing: `Yes`
- Standalone API: `Yes`
- Nx Cloud: `Skip` or `No`
- Package manager: `npm`

**This creates:**
```
src/frontend/
├── apps/
│   └── public-portal/    # First Angular app
├── libs/
├── nx.json
├── package.json
└── tsconfig.base.json
```

---

### Step 2: Create Call Center Portal App

```bash
cd src/frontend

# Generate second Angular application
npx nx generate @nx/angular:application call-center-portal
```

**When prompted:**
- Routing: `Yes`
- Stylesheet: `css`
- Standalone: `Yes`

**Result:**
```
apps/
├── public-portal/
└── call-center-portal/    # Second Angular app
```

---

### Step 3: Create Shared Libraries

```bash
cd src/frontend

# 1. Shared UI components
npx nx generate @nx/angular:library shared-ui --directory=libs/shared-ui --standalone

# 2. Data access (API clients, state)
npx nx generate @nx/angular:library data-access --directory=libs/data-access --standalone

# 3. Utilities (helpers, formatters)
npx nx generate @nx/angular:library util --directory=libs/util --standalone
```

**Result:**
```
libs/
├── shared-ui/       # Reusable components
├── data-access/     # API services, state
└── util/            # Helper functions
```

---

### Step 4: Install Tailwind CSS

```bash
cd src/frontend

# Install Tailwind
npm install -D tailwindcss postcss autoprefixer

# Generate config
npx tailwindcss init -p
```

**Create `tailwind.config.base.js`:**

```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    'apps/**/*.{html,ts}',
    'libs/**/*.{html,ts}',
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#fef6ee',
          100: '#fdebd6',
          200: '#fad3ac',
          300: '#f6b478',
          400: '#f28b41',
          500: '#ef6c1b',  // Orange brand color
          600: '#e05211',
          700: '#b93d10',
          800: '#933214',
          900: '#762b13',
        },
      },
    },
  },
  plugins: [],
};
```

**Update `apps/public-portal/src/styles.css`:**

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

/* German formatting */
html {
  font-family: system-ui, -apple-system, sans-serif;
}
```

**Update `apps/call-center-portal/src/styles.css` the same way.**

---

### Step 5: Test Frontend Setup

```bash
cd src/frontend

# Test public portal
npx nx serve public-portal
# Should open: http://localhost:4200

# Test call center portal (new terminal)
npx nx serve call-center-portal
# Should open: http://localhost:4201
```

**If both start successfully, you're done!** ✅

---

## Alternative: Simplified Manual Setup

If the Nx setup is problematic, create a simpler structure:

### 1. Create Basic Structure

```bash
cd src
mkdir -p frontend/apps frontend/libs

# Initialize npm
cd frontend
npm init -y
```

### 2. Install Angular and Nx

```bash
npm install -D nx @nx/angular @angular/cli
npm install @angular/core @angular/common @angular/platform-browser
```

### 3. Create Nx Configuration

Create `nx.json`:
```json
{
  "$schema": "./node_modules/nx/schemas/nx-schema.json",
  "targetDefaults": {
    "build": {
      "cache": true
    }
  }
}
```

### 4. Generate Apps Manually

```bash
npx nx generate @nx/angular:application public-portal
npx nx generate @nx/angular:application call-center-portal
```

---

## Verify Setup

After setup completes, verify:

```bash
cd src/frontend

# List all projects
npx nx show projects
# Should show: public-portal, call-center-portal, shared-ui, data-access, util

# Build all
npx nx run-many --target=build --all

# Test all
npx nx run-many --target=test --all
```

---

## Troubleshooting

### Problem: "Command not found: nx"

```bash
cd src/frontend
npm install
```

### Problem: Interactive prompts blocking setup

Press `Ctrl+C` to cancel, then:
```bash
# Use environment variables to skip prompts
export NX_INTERACTIVE=false
npx create-nx-workspace@latest frontend --preset=angular-monorepo --appName=public-portal --nxCloud=skip
```

### Problem: "Cannot find module @nx/angular"

```bash
cd src/frontend
npm install -D @nx/angular
```

---

## Expected Final Structure

```
src/
├── backend/                     ✅ Complete (35 projects)
└── frontend/                    ⏳ Create this
    ├── apps/
    │   ├── public-portal/
    │   │   ├── src/
    │   │   │   ├── app/
    │   │   │   ├── assets/
    │   │   │   ├── index.html
    │   │   │   ├── main.ts
    │   │   │   └── styles.css
    │   │   ├── project.json
    │   │   └── tsconfig.json
    │   │
    │   └── call-center-portal/
    │       └── (same structure)
    │
    ├── libs/
    │   ├── shared-ui/
    │   │   └── src/lib/
    │   │       ├── components/
    │   │       │   ├── button/
    │   │       │   ├── input/
    │   │       │   └── card/
    │   │       └── index.ts
    │   │
    │   ├── data-access/
    │   │   └── src/lib/
    │   │       ├── services/
    │   │       │   ├── vehicle.service.ts
    │   │       │   └── reservation.service.ts
    │   │       └── stores/
    │   │
    │   └── util/
    │       └── src/lib/
    │           ├── date-utils.ts
    │           ├── currency-formatter.ts
    │           └── validators.ts
    │
    ├── nx.json
    ├── package.json
    ├── tsconfig.base.json
    ├── tailwind.config.base.js
    └── README.md
```

---

## Once Setup Complete

### Create First Component

```bash
cd src/frontend

# Generate vehicle search component
npx nx generate @nx/angular:component vehicle-search \
  --project=public-portal \
  --path=apps/public-portal/src/app/features/vehicle-search \
  --standalone
```

### Create API Service

```bash
# Generate vehicle service
npx nx generate @nx/angular:service vehicle \
  --project=data-access \
  --path=libs/data-access/src/lib/services
```

**Example service:**

```typescript
// libs/data-access/src/lib/services/vehicle.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface Vehicle {
  id: string;
  name: string;
  categoryName: string;
  seats: number;
  fuelType: string;
  transmissionType: string;
  dailyRateNet: number;
  dailyRateGross: number;
  currency: string;
}

@Injectable({ providedIn: 'root' })
export class VehicleService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/vehicles';

  async search(criteria: any): Promise<Vehicle[]> {
    return this.http.get<Vehicle[]>(this.apiUrl, { params: criteria }).toPromise();
  }

  async getById(id: string): Promise<Vehicle> {
    return this.http.get<Vehicle>(`${this.apiUrl}/${id}`).toPromise();
  }
}
```

---

## Next: Start Feature Development

Once frontend is set up, implement **User Story 1: Vehicle Search**

**Backend:**
1. Implement Vehicle aggregate in Fleet.Domain
2. Create SearchVehiclesQuery in Fleet.Application
3. Add GET /api/vehicles endpoint in Fleet.Api

**Frontend:**
1. Create vehicle search component
2. Add search form with date pickers
3. Display vehicle cards with Tailwind
4. Connect to backend API

**Full code examples are in `SETUP_COMPLETE.md`!**

---

## Summary

**What to do:**
1. Open terminal in `src` directory
2. Run: `npx create-nx-workspace@latest frontend`
3. Answer prompts: angular-monorepo, public-portal, css, yes, yes, skip, npm
4. Wait 5-10 minutes for installation
5. Follow steps 2-5 above

**If you get stuck:** The backend is complete and working. The frontend setup is standard Nx/Angular - any Nx documentation will help!

Good luck! 🚀
