# Microfrontend Testing Guide

## Overview

This guide provides step-by-step instructions for testing the Orange Car Rental microfrontend architecture.

---

## Architecture Summary

```
Shell (4300) ──┬──> Public Portal (4301)
               └──> Call Center Portal (4302)
```

- **Shell**: Host application that loads remotes dynamically
- **Public Portal**: Customer-facing features (remote)
- **Call Center Portal**: Administrative features (remote)

---

## Prerequisites

✅ All dependencies installed (completed)
- `src/frontend/apps/shell` - 725 packages
- `src/frontend/apps/public-portal` - 800 packages
- `src/frontend/apps/call-center-portal` - 725 packages

---

## Testing Steps

### Option 1: Quick Test (Single Terminal with Concurrently)

**From Frontend Root:**
```bash
cd src/frontend
npm install concurrently -D  # If not already installed
npx concurrently \
  "cd apps/shell && npm start" \
  "cd apps/public-portal && npm run start:dev" \
  "cd apps/call-center-portal && npm run start:dev"
```

### Option 2: Manual Test (3 Terminals)

**Terminal 1 - Shell:**
```bash
cd src/frontend/apps/shell
npm start
```
Open: http://localhost:4300

**Terminal 2 - Public Portal:**
```bash
cd src/frontend/apps/public-portal
npm run start:dev
```
Open: http://localhost:4301

**Terminal 3 - Call Center Portal:**
```bash
cd src/frontend/apps/call-center-portal
npm run start:dev
```
Open: http://localhost:4302

---

## Verification Checklist

### 1. Shell Application (http://localhost:4300)

**Expected:**
- ✅ Orange header with "Orange Car Rental"
- ✅ Navigation links: Home, Vehicles, Booking, Admin
- ✅ Welcome message
- ✅ Two cards: "Public Portal" and "Call Center Portal"
- ✅ Architecture information displayed

**Test Navigation:**
- Click "Home" → should show welcome page
- Click "Vehicles" → should load public portal (check console for federation)
- Click "Booking" → should load public portal routes
- Click "Admin" → should load call center portal (check console for federation)

### 2. Public Portal Standalone (http://localhost:4301)

**Expected:**
- ✅ Vehicle list component loads
- ✅ Application runs independently
- ✅ No Module Federation errors in console

**Routes to Test:**
- `/` - Vehicle list
- `/login` - Login page
- `/register` - Registration page
- `/booking` - Booking page (requires auth)

### 3. Call Center Portal Standalone (http://localhost:4302)

**Expected:**
- ✅ Vehicles component loads
- ✅ Application runs independently
- ✅ No Module Federation errors in console

**Routes to Test:**
- `/` - Vehicles (requires auth)
- `/reservations` - Reservations management
- `/customers` - Customer management
- `/locations` - Location management

### 4. Module Federation Integration

**Browser Console Checks:**

When navigating from Shell to remotes:

**Expected Console Messages:**
```
[Shell] Loading remote: publicPortal
[Shell] Federation initialized
[Remote] Public Portal loaded successfully
```

**Check Network Tab:**
- Look for `remoteEntry.json` requests
- Verify remote chunks are loading from correct ports
- Check for any CORS issues

**DevTools Application Tab:**
- Inspect Service Workers (should be none for MF)
- Check Local Storage for any shared state

---

## Common Issues & Solutions

### Issue 1: "Failed to load remote module"

**Symptoms:**
- Error boundary shown in shell
- Console error: `Failed to fetch remoteEntry.json`

**Solution:**
1. Verify all 3 apps are running
2. Check ports: Shell (4300), Public (4301), Call Center (4302)
3. Verify `federation.manifest.json` URLs are correct

### Issue 2: CORS Errors

**Symptoms:**
- `Access-Control-Allow-Origin` errors in console

**Solution:**
- All apps run on localhost, CORS shouldn't be an issue
- If present, check if any proxy configurations are interfering

### Issue 3: Duplicate Angular Versions

**Symptoms:**
- Console warnings about multiple Angular instances
- Routing doesn't work across remotes

**Solution:**
- Verify `federation.config.js` has `singleton: true` for Angular
- Check all apps use same Angular version (20.3.0)

### Issue 4: 404 on Remote Entry

**Symptoms:**
- 404 error for `http://localhost:4301/remoteEntry.json`

**Solution:**
- Native Federation generates this file at build/serve time
- Restart the remote application
- Check angular.json has proper build configuration

---

## Advanced Testing

### 1. Hot Module Replacement (HMR)

**Test:**
1. Start all apps
2. Navigate from shell to public portal
3. Edit `public-portal/src/app/pages/vehicle-list/vehicle-list.component.ts`
4. Save file

**Expected:**
- Remote should reload
- Shell stays intact
- No full page refresh

### 2. Independent Deployment Simulation

**Test:**
1. Start shell and call-center-portal only (no public-portal)
2. Navigate to "Vehicles" in shell

**Expected:**
- Error boundary shown
- Helpful error message displayed
- Shell remains functional

### 3. Shared Dependencies

**Test:**
1. Open DevTools Network tab
2. Navigate between remotes
3. Filter by "vendor" or "chunk"

**Expected:**
- Angular core libraries loaded once (from shell)
- Only remote-specific code downloaded for each remote
- Shared chunks not re-downloaded

---

## Performance Metrics

### Initial Load Time

**Shell (First Load):**
- Target: < 2s
- Includes: Shell code + Angular core + Router

**Remote Load (via Shell):**
- Target: < 500ms
- Includes: Only remote-specific code

### Bundle Sizes

**Expected Sizes:**
- Shell: ~500KB (with shared deps)
- Public Portal (remote chunk): ~200KB
- Call Center Portal (remote chunk): ~150KB

**Check Sizes:**
```bash
# Build all apps
cd src/frontend/apps/shell && npm run build
cd ../public-portal && npm run build
cd ../call-center-portal && npm run build

# Check dist folder sizes
```

---

## Browser Compatibility

Test in multiple browsers:
- ✅ Chrome 90+
- ✅ Firefox 88+
- ✅ Edge 90+
- ✅ Safari 14+

Module Federation is supported in all modern browsers.

---

## Debugging Tips

### Enable Verbose Logging

Add to `main.ts` in each app:
```typescript
// Enable MF debug logs
(window as any).__DEBUG_NF__ = true;
```

### Inspect Federation Manifest

**Shell:**
```bash
curl http://localhost:4300/assets/federation.manifest.json
```

**Expected:**
```json
{
  "publicPortal": "http://localhost:4301/remoteEntry.json",
  "callCenterPortal": "http://localhost:4302/remoteEntry.json"
}
```

### Check Remote Entry

```bash
curl http://localhost:4301/remoteEntry.json
```

Should return a valid JSON response with module exports.

---

## Success Criteria

The microfrontend architecture is working correctly if:

1. ✅ Shell loads without errors
2. ✅ All three apps run independently
3. ✅ Shell can load both remotes dynamically
4. ✅ Navigation works seamlessly between remotes
5. ✅ No duplicate Angular instances
6. ✅ Shared dependencies loaded once
7. ✅ Error boundaries show for unavailable remotes
8. ✅ HMR works for each app independently
9. ✅ Console shows federation initialization logs
10. ✅ Network tab shows remote chunks loading

---

## Next Steps

Once basic testing is complete:

1. **Add Shared Services** - Implement EventBus for cross-app communication
2. **Production Build** - Test production builds and optimization
3. **E2E Tests** - Add Playwright tests for MF flows
4. **Deployment** - Deploy to staging environment
5. **Monitoring** - Add analytics for remote load times
6. **Documentation** - Document team conventions for MF development

---

## Troubleshooting Commands

```bash
# Check if ports are in use
netstat -ano | findstr "4200"
netstat -ano | findstr "4201"
netstat -ano | findstr "4202"

# Kill processes on ports (Windows)
taskkill /PID <PID> /F

# Clear npm cache if issues persist
npm cache clean --force

# Reinstall dependencies
rm -rf node_modules package-lock.json
npm install

# Check Angular CLI version
npx ng version
```

---

## Support

If you encounter issues:

1. Check browser console for errors
2. Verify all three apps are running
3. Review `federation.config.js` in each app
4. Check Angular version compatibility
5. Consult Native Federation docs: https://www.npmjs.com/package/@angular-architects/native-federation

---

**Document Version**: 1.0
**Last Updated**: 2025-12-27
**Status**: Ready for Testing
