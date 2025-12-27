# Concurrency - 12-Factor Principles

## Overview

This document describes the concurrency model for Orange Car Rental, following **12-Factor App - Factor 8: Scale out via the process model**.

## 12-Factor Concurrency Principles

✅ **Horizontal scaling** - Scale by adding more processes, not making processes bigger
✅ **Process types** - Different work assigned to different process types (web, worker, etc.)
✅ **Share-nothing** - Processes don't share memory (see Factor 6: Processes)
✅ **Process manager** - Never daemonize, rely on OS process manager (systemd, Kubernetes)
✅ **First-class processes** - Processes are disposable and can be started/stopped quickly

---

## What is Concurrency (Factor 8)?

**Concurrency** means the app **scales out by running multiple processes** (horizontal scaling), not by making a single process bigger (vertical scaling).

**Key Principle:** Treat **processes as first-class citizens**. Different types of work should be assigned to different **process types** (web server, background worker, etc.), and each process type can scale independently.

**12-Factor Approach:**
- **Web processes** handle HTTP requests (multiple replicas)
- **Worker processes** handle background jobs (separate deployment)
- **Scale horizontally** by adding more processes
- **Process manager** (Kubernetes) handles process lifecycle

---

## Current Implementation Status

### ✅ **Horizontal Scaling - Excellent Compliance**

**Kubernetes Deployments with Replicas:**

**Base Configuration:** `k8s/base/vehicles-api-deployment.yaml:9`

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: vehicles-api
spec:
  replicas: 3  # ✅ Three concurrent processes
  selector:
    matchLabels:
      app: vehicles-api
  template:
    spec:
      containers:
      - name: vehicles-api
        image: ghcr.io/.../vehicles-api:v1.2.3
        ports:
        - containerPort: 8080
```

**Production Scaling:** `k8s/overlays/production/kustomization.yaml:26-38`

```yaml
replicas:
- name: public-portal
  count: 3  # ✅ 3 frontend processes
- name: vehicles-api
  count: 5  # ✅ 5 API processes
- name: reservations-api
  count: 5  # ✅ 5 API processes
- name: customers-api
  count: 3  # ✅ 3 API processes
- name: locations-api
  count: 2  # ✅ 2 API processes
- name: keycloak
  count: 3  # ✅ 3 auth processes
```

**✅ Current Replica Counts:**

| Environment | Vehicles API | Reservations API | Customers API | Public Portal |
|-------------|--------------|------------------|---------------|---------------|
| **Base (Dev/Staging)** | 3 | 3 | 3 | 2 |
| **Production** | 5 | 5 | 3 | 3 |
| **HPA Max (Prod)** | 20 | - | - | - |

---

### ✅ **Horizontal Pod Autoscaler (HPA)**

**Production Autoscaling:** `k8s/overlays/production/kustomization.yaml:50-76`

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: vehicles-api
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: vehicles-api
  minReplicas: 5   # ✅ Never less than 5 processes
  maxReplicas: 20  # ✅ Scale up to 20 processes
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70  # ✅ Scale when CPU > 70%
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80  # ✅ Scale when memory > 80%
```

**✅ Autoscaling Behavior:**

```
Traffic Low (< 70% CPU)
  ├─> Kubernetes scales DOWN to 5 replicas (minReplicas)

Traffic Medium (70-80% CPU)
  ├─> Kubernetes maintains current replicas (5-10)

Traffic High (> 80% CPU)
  ├─> Kubernetes scales UP to 20 replicas (maxReplicas)

Traffic Peak (Black Friday, holidays)
  ├─> All 20 replicas running
  ├─> Each handling ~100 req/sec
  ├─> Total capacity: 2000 req/sec
```

**Scaling Decision Flow:**
```
┌────────────────────────────┐
│  Metrics Server            │
│  (collects CPU/memory)     │
└───────┬────────────────────┘
        │ Every 15 seconds
        ▼
┌────────────────────────────┐
│  HPA Controller            │
│  • Check current metrics   │
│  • Compare to target       │
│  • Calculate desired count │
└───────┬────────────────────┘
        │
        ├─> CPU > 70% → Scale UP (add pods)
        ├─> CPU < 50% → Scale DOWN (remove pods)
        └─> Within range → No change
        │
        ▼
┌────────────────────────────┐
│  Deployment Controller     │
│  • Create/delete pods      │
│  • Wait for health checks  │
│  • Update Service endpoints│
└────────────────────────────┘
```

---

### ✅ **Resource Allocation**

**CPU and Memory Requests/Limits:** `k8s/base/vehicles-api-deployment.yaml:54-60`

```yaml
resources:
  requests:     # ✅ Minimum guaranteed resources
    memory: "512Mi"   # 0.5 GB
    cpu: "500m"       # 0.5 CPU cores
  limits:       # ✅ Maximum allowed resources
    memory: "1Gi"     # 1 GB
    cpu: "1000m"      # 1 CPU core
```

**✅ Resource Strategy:**

| Metric | Request | Limit | Reasoning |
|--------|---------|-------|-----------|
| **CPU** | 500m (0.5 core) | 1000m (1 core) | Normal load uses 0.5 core, bursts to 1 core |
| **Memory** | 512Mi | 1Gi | .NET runtime needs 512Mi, limit prevents OOM |

**Benefits:**
- **Requests** ensure Kubernetes schedules pods on nodes with enough resources
- **Limits** prevent runaway processes from consuming all node resources
- **2:1 ratio** allows bursting during traffic spikes

**Capacity Planning Example:**
```
Single Node (4 CPUs, 16 GB RAM)
  ├─> CPU capacity: 4000m
  │   └─> Vehicles API requests: 500m each
  │       └─> Can schedule: 8 pods per node
  │
  └─> Memory capacity: 16 GB
      └─> Vehicles API requests: 512Mi each
          └─> Can schedule: 31 pods per node

Bottleneck: CPU (8 pods per node)
```

---

## Process Types

### **Web Processes (HTTP Servers)**

**Current Process Types:**

| Process Type | Purpose | Port | Replicas (Prod) |
|--------------|---------|------|-----------------|
| **Fleet API** | Vehicle inventory | 8080 | 5 |
| **Reservations API** | Booking management | 8080 | 5 |
| **Customers API** | Customer profiles | 8080 | 3 |
| **Pricing API** | Rate calculation | 8080 | 3 |
| **Locations API** | Location management | 8080 | 2 |
| **Payments API** | Payment processing | 8080 | 3 |
| **Notifications API** | Email/SMS sending | 8080 | 2 |
| **API Gateway** | Reverse proxy | 8080 | 3 |
| **Public Portal** | Customer frontend | 80 | 3 |
| **Call Center Portal** | Agent frontend | 80 | 2 |

**✅ Benefits of Multiple Process Types:**
- Each service scales independently
- Reservations API can scale to 20 replicas during booking rush
- Locations API stays at 2 replicas (low traffic)
- Resource-efficient (don't over-provision all services)

---

### **Worker Processes (Background Jobs)** ⚠️ Not Implemented Yet

**Recommended Implementation:**

**Use Case: Email Notification Worker**

```yaml
# k8s/base/notification-worker-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: notification-worker
spec:
  replicas: 2  # ✅ Separate scaling from web processes
  template:
    spec:
      containers:
      - name: notification-worker
        image: ghcr.io/.../notification-worker:v1.0.0
        env:
        - name: WORKER_TYPE
          value: "email-sender"
        - name: QUEUE_URL
          value: "amqp://rabbitmq:5672"
```

**Worker Process (C#):**

```csharp
// NotificationWorker/Program.cs
public class EmailWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailQueue = scope.ServiceProvider.GetRequiredService<IEmailQueue>();

            var message = await emailQueue.DequeueAsync(stoppingToken);
            if (message != null)
            {
                await SendEmailAsync(message);
            }

            await Task.Delay(100, stoppingToken);  // Poll every 100ms
        }
    }
}
```

**✅ Benefits:**
- Email sending doesn't block HTTP requests
- Worker processes scale independently (can run 10 workers while web has 5)
- Failures don't affect web processes
- Can deploy/restart without affecting API availability

**📋 Recommended Process Types to Add:**

1. **Email Worker** - Send queued emails (Factor 12: Admin Processes)
2. **Report Generator** - Generate monthly reports
3. **Data Sync Worker** - Sync with external systems
4. **Cleanup Worker** - Delete expired sessions, old logs

---

## Scaling Strategies

### **1. Horizontal Scaling (Current)** ✅

**Add more processes of the same type:**

```bash
# Scale Vehicles API from 5 to 10 replicas
kubectl scale deployment vehicles-api --replicas=10

# Verify scaling
kubectl get pods -l app=vehicles-api

# Output:
NAME                           READY   STATUS    RESTARTS
vehicles-api-5f8b7d9c4-abc12   1/1     Running   0
vehicles-api-5f8b7d9c4-def34   1/1     Running   0
... (10 total pods)
```

**✅ Benefits:**
- Linear scaling (10 processes = 10x capacity)
- No code changes required
- Easy to automate (HPA)
- Cost-effective (add/remove as needed)

---

### **2. Vertical Scaling (Anti-Pattern)** ❌

**Make a single process bigger:**

```yaml
# ❌ DON'T DO THIS
resources:
  requests:
    cpu: "4000m"      # 4 full CPU cores
    memory: "8Gi"     # 8 GB RAM
```

**❌ Problems:**
- Doesn't improve concurrency (still 1 process)
- Expensive (larger VM required)
- Single point of failure
- Harder to replace/restart (longer downtime)
- Doesn't align with cloud-native architecture

**✅ 12-Factor Approach:** Use horizontal scaling instead (more small processes).

---

### **3. Auto-Scaling (Production)** ✅

**Kubernetes HPA automatically adjusts replica count:**

**Scaling Events:**
```
09:00 - Low traffic (100 req/min)
  └─> HPA: 5 replicas (minReplicas)

12:00 - Lunch rush (500 req/min)
  ├─> CPU increases to 75%
  └─> HPA: Scale to 8 replicas

14:00 - Traffic drops (200 req/min)
  ├─> CPU decreases to 50%
  └─> HPA: Scale down to 5 replicas

18:00 - Evening rush (1000 req/min)
  ├─> CPU increases to 85%
  └─> HPA: Scale to 15 replicas

23:00 - Night (50 req/min)
  ├─> CPU decreases to 40%
  └─> HPA: Scale down to 5 replicas
```

**Metrics-Based Scaling:**

```yaml
metrics:
- type: Resource
  resource:
    name: cpu
    target:
      averageUtilization: 70  # Target 70% CPU

# Calculation:
# Current CPU: 85%
# Target CPU: 70%
# Desired replicas = current replicas * (85 / 70) = 5 * 1.21 = 6 (rounded)
```

---

### **4. Scheduled Scaling** 📋 Recommended

**Use CronJobs to pre-scale before expected traffic:**

**Example: Scale up before Black Friday sale (9 AM Friday):**

```yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: scale-up-for-sale
spec:
  schedule: "0 8 * * 5"  # 8 AM every Friday
  jobTemplate:
    spec:
      template:
        spec:
          containers:
          - name: scaler
            image: bitnami/kubectl
            command:
            - /bin/sh
            - -c
            - kubectl scale deployment vehicles-api --replicas=20
          restartPolicy: OnFailure
```

**✅ Benefits:**
- Proactive scaling (not reactive)
- Capacity ready before traffic spike
- Better user experience (no slow scale-up period)

---

## No Daemon Processes

### ✅ **Process Manager: Kubernetes**

**12-Factor Principle:** Never daemonize or write PID files. Use the OS process manager.

**Current Approach (Correct):**

```yaml
# k8s/base/vehicles-api-deployment.yaml
apiVersion: apps/v1
kind: Deployment
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: vehicles-api
        # ✅ Kubernetes manages process lifecycle
        # No PID files
        # No daemonization
        # No supervisord/systemd needed
```

**Kubernetes Process Management:**
```
┌─────────────────────────────────┐
│  kubelet (on each node)         │
│  • Starts containers            │
│  • Monitors health checks       │
│  • Restarts failed containers   │
│  • Reports status to API server │
└─────────────────────────────────┘
```

**✅ Benefits:**
- Kubernetes handles restarts automatically
- No need for custom process managers
- Centralized logging (stdout → Kubernetes)
- Consistent across all services

---

### ❌ **Anti-Pattern: Daemonization**

**What NOT to do:**

```bash
# ❌ DON'T write PID files
echo $$ > /var/run/app.pid

# ❌ DON'T daemonize
nohup dotnet App.dll &

# ❌ DON'T use supervisord in containers
supervisord -c /etc/supervisor/supervisord.conf
```

**Why?**
- Kubernetes can't monitor daemonized processes
- Health checks won't work correctly
- Graceful shutdown (SIGTERM) won't propagate
- Logs won't go to stdout (Kubernetes can't collect them)

**✅ Correct Approach:**

```dockerfile
# Dockerfile
ENTRYPOINT ["dotnet", "OrangeCarRental.Fleet.Api.dll"]
# ✅ Process runs in foreground
# ✅ Kubernetes gets PID 1
# ✅ SIGTERM goes directly to app
```

---

## Load Balancing

### **Kubernetes Service (ClusterIP)**

```yaml
# k8s/base/vehicles-api-deployment.yaml:78-88
apiVersion: v1
kind: Service
metadata:
  name: fleet-api
spec:
  type: ClusterIP
  selector:
    app: fleet-api  # ✅ Routes to all pods with this label
  ports:
  - port: 8080
    targetPort: 8080
```

**Load Balancing Algorithm:**
```
┌───────────────────────────────┐
│  Service: fleet-api           │
│  ClusterIP: 10.0.1.100:8080   │
└───────┬───────────────────────┘
        │
        ├─> Round-robin (default)
        │
        ├───────┬───────┬───────┬───────┐
        ▼       ▼       ▼       ▼       ▼
    Pod 1   Pod 2   Pod 3   Pod 4   Pod 5
    (8080)  (8080)  (8080)  (8080)  (8080)
```

**Request Distribution:**
```
Request 1 → Pod 1
Request 2 → Pod 2
Request 3 → Pod 3
Request 4 → Pod 4
Request 5 → Pod 5
Request 6 → Pod 1 (round-robin)
```

**✅ Benefits:**
- Even distribution across all pods
- Automatic health check integration (unhealthy pods removed)
- No sticky sessions (stateless processes from Factor 6)
- Zero-downtime deployments (rolling updates)

---

## Best Practices Checklist

### **Horizontal Scaling**
- [x] Multiple replicas configured (3-5 per service)
- [x] Kubernetes manages process lifecycle
- [x] No vertical scaling (same resources for all pods)
- [x] Load balancing via Kubernetes Service
- [ ] **TODO:** Implement HPA for all services (currently only vehicles-api)

### **Resource Management**
- [x] CPU requests/limits defined (500m/1000m)
- [x] Memory requests/limits defined (512Mi/1Gi)
- [x] 2:1 ratio allows bursting
- [x] Resource quotas prevent overconsumption
- [x] Liveness and readiness probes configured

### **Process Types**
- [x] Web processes (APIs, frontends)
- [ ] **TODO:** Worker processes for background jobs
- [x] Each process type can scale independently
- [x] No process type dependencies

### **Autoscaling**
- [x] HPA configured for production
- [x] Metrics-based scaling (CPU, memory)
- [ ] **TODO:** Custom metrics (request rate, queue depth)
- [ ] **TODO:** Scheduled scaling for known traffic patterns

### **Process Management**
- [x] No daemon processes (Kubernetes manages)
- [x] No PID files
- [x] Processes run in foreground
- [x] SIGTERM handled correctly (graceful shutdown)

---

## Recommendations

### **1. Implement HPA for All Services** 🟡 Medium Priority

**Current:** Only `vehicles-api` has HPA.

**Recommendation:** Add HPA for all production services.

**Example: Reservations API HPA**

```yaml
# k8s/overlays/production/reservations-api-hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: reservations-api
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: reservations-api
  minReplicas: 5
  maxReplicas: 15
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        averageUtilization: 80
```

**Benefits:**
- All services scale automatically
- Consistent scaling behavior
- Better resource utilization

---

### **2. Add Worker Processes** 🟡 Medium Priority

**Current:** No background worker processes.

**Use Cases:**
1. **Email Worker** - Process queued emails
2. **Report Worker** - Generate monthly reports
3. **Cleanup Worker** - Archive old reservations

**Implementation:**

```csharp
// Workers/EmailWorker/Program.cs
var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddHostedService<EmailWorkerService>();
    services.AddScoped<IEmailQueue, RabbitMqEmailQueue>();
});

var app = builder.Build();
app.Run();
```

**Kubernetes Deployment:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: email-worker
spec:
  replicas: 3  # ✅ Independent scaling from web processes
  template:
    spec:
      containers:
      - name: email-worker
        image: ghcr.io/.../email-worker:v1.0.0
        env:
        - name: RABBITMQ_URL
          valueFrom:
            secretKeyRef:
              name: rabbitmq-secrets
              key: url
```

---

### **3. Custom Metrics for HPA** 🟢 Low Priority

**Current:** HPA uses CPU and memory only.

**Recommendation:** Add custom metrics (request rate, queue depth).

**Example: Scale based on HTTP request rate:**

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
spec:
  metrics:
  - type: Pods
    pods:
      metric:
        name: http_requests_per_second
      target:
        type: AverageValue
        averageValue: "100"  # Scale when > 100 req/sec per pod
```

**Benefits:**
- More accurate scaling (business metrics)
- Proactive scaling (before CPU/memory saturate)
- Better user experience

---

## Verification Commands

### **Check Current Replicas**
```bash
kubectl get deployments

# Output:
NAME               READY   UP-TO-DATE   AVAILABLE   AGE
vehicles-api       5/5     5            5           10d
reservations-api   5/5     5            5           10d
customers-api      3/3     3            3           10d
```

### **Scale Manually**
```bash
# Scale to 10 replicas
kubectl scale deployment vehicles-api --replicas=10

# Verify
kubectl get pods -l app=vehicles-api

# Watch scaling in real-time
kubectl get pods -l app=vehicles-api --watch
```

### **Check HPA Status**
```bash
kubectl get hpa

# Output:
NAME           REFERENCE                 TARGETS         MINPODS   MAXPODS   REPLICAS   AGE
vehicles-api   Deployment/vehicles-api   45%/70%, 60%/80%   5         20        5          10d
#                                         CPU    Memory
```

### **View HPA Events**
```bash
kubectl describe hpa vehicles-api

# Shows scaling events:
# - Scaled up from 5 to 8 (CPU: 75% > 70%)
# - Scaled down from 8 to 5 (CPU: 50% < 70%)
```

### **Test Load Balancing**
```bash
# Send 100 requests and check which pods handle them
for i in {1..100}; do
  kubectl exec -it test-pod -- curl -s http://fleet-api:8080/api/vehicles | grep "pod-name"
done | sort | uniq -c

# Output (even distribution):
# 20 pod-1
# 20 pod-2
# 20 pod-3
# 20 pod-4
# 20 pod-5
```

---

## Additional Resources

- [12-Factor App - Concurrency](https://12factor.net/concurrency)
- [Kubernetes Horizontal Pod Autoscaler](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/)
- [Kubernetes Resource Management](https://kubernetes.io/docs/concepts/configuration/manage-resources-containers/)
- [HPA Walkthrough](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale-walkthrough/)

---

## Summary

| Aspect | Status | Compliance |
|--------|--------|------------|
| **Horizontal Scaling** | ✅ Implemented | Excellent (3-5 replicas) |
| **HPA (Autoscaling)** | ⚠️ Partial | Good (vehicles-api only) |
| **Resource Limits** | ✅ Configured | Excellent (CPU/memory) |
| **Process Types** | ⚠️ Web Only | Good (needs workers) |
| **No Daemons** | ✅ Verified | Excellent (K8s managed) |
| **Load Balancing** | ✅ Implemented | Excellent (Service) |
| **Independent Scaling** | ✅ Implemented | Excellent (per service) |

**Overall Compliance:** ✅ **Excellent** - Factor 8 implemented with horizontal scaling and autoscaling.

**Next Steps:**
1. Add HPA for all services (Medium Priority)
2. Implement worker processes for background jobs (Medium Priority)
3. Add custom metrics for HPA (Low Priority)
