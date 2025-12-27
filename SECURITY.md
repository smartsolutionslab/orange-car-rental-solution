# Security Policy

## Supported Versions

We release patches for security vulnerabilities in the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 1.1.x   | :white_check_mark: |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take security vulnerabilities seriously. If you discover a security issue, please follow these steps:

### 1. Do Not Disclose Publicly

Please **do not** create a public GitHub issue for security vulnerabilities.

### 2. Report Privately

Send your findings to: **security@orangecarrental.com**

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)
- Your contact information

### 3. Response Timeline

- **Initial Response:** Within 48 hours
- **Assessment:** Within 5 business days
- **Fix Timeline:** Depends on severity
  - Critical: 7 days
  - High: 14 days
  - Medium: 30 days
  - Low: Next release

### 4. Disclosure Process

1. We confirm the vulnerability and scope
2. We develop and test a fix
3. We release the fix
4. We publish a security advisory
5. We credit you in the advisory (if desired)

## Security Best Practices

### For Contributors

When contributing code:

1. **No Secrets in Code**
   - Never commit API keys, passwords, or tokens
   - Use environment variables
   - Review code before committing

2. **Input Validation**
   - Validate all user inputs
   - Sanitize data before processing
   - Use parameterized queries

3. **Authentication & Authorization**
   - Follow OAuth 2.0 / OIDC standards
   - Implement proper session management
   - Use JWT tokens securely

4. **Dependencies**
   - Keep dependencies updated
   - Review security advisories
   - Use `npm audit` regularly

5. **Error Handling**
   - Don't expose sensitive information in errors
   - Log security events
   - Handle errors gracefully

### For Deployment

1. **HTTPS Only**
   - Enforce HTTPS in production
   - Use valid SSL certificates
   - Enable HSTS

2. **Environment Variables**
   - Store secrets in secure vaults
   - Rotate credentials regularly
   - Limit access to production secrets

3. **Database Security**
   - Use encrypted connections
   - Implement row-level security
   - Regular backups
   - Restrict database access

4. **Monitoring**
   - Log security events
   - Monitor for suspicious activity
   - Set up alerts
   - Regular security audits

## Known Security Considerations

### Authentication

- Uses Keycloak for SSO
- JWT tokens with expiration
- Refresh token rotation
- Password complexity requirements

### Data Protection

- Personal data encrypted at rest
- Secure transmission (TLS 1.3)
- GDPR compliance measures
- Data retention policies

### API Security

- Rate limiting
- CORS configuration
- Input validation
- SQL injection prevention
- XSS protection

## Security Checklist for PRs

Before merging, ensure:

- [ ] No secrets in code
- [ ] Input validation implemented
- [ ] SQL injection prevented
- [ ] XSS vulnerabilities addressed
- [ ] Authentication/authorization checked
- [ ] Dependencies updated
- [ ] Security tests added
- [ ] Error messages don't leak info

## Automated Security Checks

We use the following tools:

- **npm audit** - Dependency vulnerability scanning
- **SonarQube** - Code quality and security
- **OWASP ZAP** - Security testing (planned)
- **Dependabot** - Automated dependency updates

## Security Updates

Subscribe to security advisories:

- GitHub Security Advisories
- Project mailing list
- Release notes

## Contact

For security concerns:
- **Email:** security@orangecarrental.com
- **PGP Key:** [Link to PGP public key]

For general questions:
- **GitHub Discussions:** https://github.com/orangecarrental/discussions
- **Email:** support@orangecarrental.com

---

**Last Updated:** 2025-11-21

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
