# Development Scripts

This directory contains helpful automation scripts for local development, testing, and deployment.

## 📁 Available Scripts

### Setup Scripts

#### `setup-local.sh` / `setup-local.ps1`
Automated local development environment setup.

**What it does:**
- Checks Node.js and .NET installation
- Installs root dependencies
- Installs Playwright browsers
- Sets up both frontend portals
- Restores backend dependencies
- Creates .env file from template

**Usage:**

```bash
# Linux/macOS
npm run setup

# Windows
npm run setup:windows
```

**Time:** ~5-10 minutes depending on internet speed

---

### Validation Scripts

#### `pre-commit-check.sh`
Pre-commit validation to ensure code quality before committing.

**What it checks:**
1. Linting (both portals)
2. Unit tests (both portals)
3. console.log statements
4. TODO/FIXME comments

**Usage:**

```bash
npm run precommit
```

**When to use:**
- Before committing changes
- As part of git hooks (future enhancement)
- Before creating pull requests

**Time:** ~3-5 minutes

---

### Analysis Scripts

#### `analyze-bundle.sh`
Analyzes production bundle size and composition.

**What it does:**
- Builds production bundles for both portals
- Generates bundle statistics
- Creates interactive bundle reports (if webpack-bundle-analyzer is available)
- Shows bundle sizes and main files

**Usage:**

```bash
npm run analyze:bundle
```

**Output:**
- `bundle-report-public.html` - Public portal bundle analysis
- `bundle-report-callcenter.html` - Call center portal bundle analysis

**When to use:**
- Before releasing to production
- When optimizing bundle size
- When investigating performance issues
- After adding new dependencies

**Time:** ~2-3 minutes

---

## 🚀 Quick Reference

### First-time Setup

```bash
# 1. Clone the repository
git clone <repository-url>
cd orange-car-rental

# 2. Run setup script
npm run setup              # Linux/macOS
npm run setup:windows      # Windows

# 3. Edit .env with your credentials
code .env

# 4. Verify installation
npm run test:unit
```

### Before Every Commit

```bash
# Run pre-commit checks
npm run precommit

# If all checks pass, commit
git add .
git commit -m "your message"
```

### Performance Analysis

```bash
# Analyze bundle size
npm run analyze:bundle

# Open the generated reports
open bundle-report-public.html
open bundle-report-callcenter.html
```

---

## 📝 Script Permissions

All shell scripts need execute permissions on Linux/macOS:

```bash
chmod +x scripts/*.sh
```

For Windows PowerShell scripts, you may need to adjust execution policy:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## 🔧 Customization

### Adding New Checks to Pre-commit

Edit `pre-commit-check.sh` and add your checks:

```bash
# Check X: Your custom check
echo -e "${YELLOW}[X/Y] Running custom check...${NC}"
if your-command; then
    echo -e "${GREEN}✓ Custom check passed${NC}"
else
    echo -e "${RED}✗ Custom check failed${NC}"
    FAILED=1
fi
```

### Adding Setup Steps

Edit `setup-local.sh` or `setup-local.ps1` and add your steps:

```bash
echo -e "${CYAN}Step X: Your Setup Step${NC}"
# Your setup commands here
echo -e "${GREEN}✓ Your step completed${NC}"
```

---

## 🐛 Troubleshooting

### Setup script fails on Windows

**Issue:** PowerShell execution policy blocks the script

**Solution:**
```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
npm run setup:windows
```

### Pre-commit check fails on linting

**Issue:** ESLint errors in code

**Solution:**
```bash
# Auto-fix linting issues
npm run format

# Check remaining issues
npm run lint
```

### Bundle analysis fails

**Issue:** webpack-bundle-analyzer not installed

**Solution:**
```bash
# Install globally
npm install -g webpack-bundle-analyzer

# Or use without analyzer (just shows sizes)
# The script will still work and show basic bundle info
```

### Permission denied on Linux/macOS

**Issue:** Scripts don't have execute permission

**Solution:**
```bash
chmod +x scripts/*.sh
```

---

## 📊 Script Output Examples

### Setup Script Success

```
=====================================
Orange Car Rental - Local Setup
=====================================

✓ Node.js version: v20.10.0
✓ .NET version: 8.0.100

=====================================
Step 1: Installing Root Dependencies
=====================================
✓ Root dependencies installed

[... more steps ...]

Setup Complete!
```

### Pre-commit Check Success

```
======================================
Pre-Commit Validation
======================================

[1/4] Running linting...
✓ Public portal linting passed
✓ Call center portal linting passed

[2/4] Running unit tests...
✓ Public portal unit tests passed
✓ Call center portal unit tests passed

[3/4] Checking for console.log statements...
✓ No console.log statements found

[4/4] Checking for TODO comments...
✓ No TODO/FIXME comments found

======================================
Pre-commit validation PASSED
Ready to commit!
```

---

## 🔗 Related Documentation

- **Setup Guide:** See [QUICK-START-TESTING.md](../QUICK-START-TESTING.md)
- **Testing:** See [TEST-COVERAGE-REPORT.md](../TEST-COVERAGE-REPORT.md)
- **CI/CD:** See [CI-CD-SETUP.md](../CI-CD-SETUP.md)
- **Deployment:** See [WHATS-NEXT.md](../WHATS-NEXT.md)

---

## 💡 Tips

1. **Run setup once** - You only need to run the setup script once when first setting up the project
2. **Use precommit regularly** - Make it a habit to run before every commit
3. **Analyze bundles periodically** - Check bundle size monthly or when adding dependencies
4. **Keep scripts updated** - As the project evolves, update scripts to match new requirements

---

## 🤝 Contributing

When adding new scripts:

1. Create both `.sh` (Linux/macOS) and `.ps1` (Windows) versions
2. Add npm script commands in root `package.json`
3. Document the script in this README
4. Make scripts idempotent (safe to run multiple times)
5. Include error handling and clear output messages
6. Test on both Windows and Linux/macOS

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
