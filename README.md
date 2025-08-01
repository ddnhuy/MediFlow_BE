# 🏥 MediFlow

<div align="center">

![MediFlow Logo](https://img.shields.io/badge/MediFlow-Hospital%20Management-blue?style=for-the-badge&logo=medical-cross)

**A modern hospital workflow management system focusing on vaccination and medical examination processes**

[![Build Status](https://img.shields.io/github/actions/workflow/status/ddnhuy/MediFlow_BE/develop.yml?branch=develop&style=flat-square&logo=github)](https://github.com/ddnhuy/MediFlow_BE/actions)
[![Code Quality](https://img.shields.io/badge/SonarQube-Passed-green?style=flat-square&logo=sonarqube)](https://sonarcloud.io/)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue?style=flat-square&logo=docker)](https://hub.docker.com/)
[![Kubernetes](https://img.shields.io/badge/Kubernetes-Supported-blue?style=flat-square&logo=kubernetes)](https://kubernetes.io/)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](LICENSE.txt)

</div>

---

## 📋 Overview

**MediFlow** is designed with a **microservices architecture** to ensure scalability, maintainability, and seamless integration with existing healthcare systems. This project serves as the graduation thesis for a team of software engineering students, demonstrating enterprise-level development practices and modern cloud-native technologies.

## 🛠️ Technology Stack

<table>
<tr>
<td valign="top" width="50%">

### Backend & Core
- ![.NET](https://img.shields.io/badge/.NET_8.0-512BD4?style=flat-square&logo=dotnet) **ASP.NET Core Web API**
- ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat-square&logo=postgresql&logoColor=white) **Database**
- ![Redis](https://img.shields.io/badge/Redis-DC382D?style=flat-square&logo=redis&logoColor=white) **Caching**
- ![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=flat-square&logo=rabbitmq&logoColor=white) **Message Queue**
- ![gRPC](https://img.shields.io/badge/gRPC-4285F4?style=flat-square&logo=google&logoColor=white) **Communication**

### DevOps & Infrastructure
- ![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat-square&logo=docker&logoColor=white) **Containerization**
- ![Kubernetes](https://img.shields.io/badge/Kubernetes-326CE5?style=flat-square&logo=kubernetes&logoColor=white) **Orchestration**
- ![Helm](https://img.shields.io/badge/Helm-0F1689?style=flat-square&logo=helm&logoColor=white) **Package Manager**
- ![GitHub Actions](https://img.shields.io/badge/GitHub_Actions-2088FF?style=flat-square&logo=github-actions&logoColor=white) **CI/CD**

</td>
<td valign="top" width="50%">

### Libraries & Frameworks
- ![YARP](https://img.shields.io/badge/YARP-512BD4?style=flat-square) **API Gateway**
- ![JWT](https://img.shields.io/badge/JWT-000000?style=flat-square&logo=jsonwebtokens&logoColor=white) **Authentication**
- ![Serilog](https://img.shields.io/badge/Serilog-3EAAAF?style=flat-square) **Logging**
- ![MassTransit](https://img.shields.io/badge/MassTransit-FF6B6B?style=flat-square) **Messaging**
- ![Mapster](https://img.shields.io/badge/Mapster-4CAF50?style=flat-square) **Object Mapping**

### Cloud & Storage
- ![AWS S3](https://img.shields.io/badge/AWS_S3-232F3E?style=flat-square&logo=amazon-aws&logoColor=white) **File Storage**
- ![Cloudinary](https://img.shields.io/badge/Cloudinary-3448C5?style=flat-square&logo=cloudinary&logoColor=white) **Media Management**
- ![SonarQube](https://img.shields.io/badge/SonarQube-4E9BCD?style=flat-square&logo=sonarqube&logoColor=white) **Code Quality**

</td>
</tr>
</table>

## 🏗️ System Architecture

<div align="center">

```mermaid
graph TB
    Client[👤 Client Applications] --> Gateway[🚪 YARP API Gateway]
    
    Gateway --> Auth[🔐 Authentication Service]
    Gateway --> Appt[📅 Appointment Service]
    Gateway --> Vacc[💉 VaccinationReception Service]
    Gateway --> Inv[📦 Inventory Service]
    Gateway --> Hosp[🏥 HospitalService]
    Gateway --> File[📁 FileStorage Service]
    Gateway --> Mgmt[⚙️ Management Service]
    
    Auth --> HR[👥 HumanResource gRPC]
    Appt --> HR
    Appt --> Cust[👤 CustomerInfo gRPC]
    Vacc --> Cust
    Mgmt --> HR
    
    Auth --> DB1[(🗄️ Auth DB)]
    HR --> DB2[(🗄️ HR DB)]
    Cust --> DB3[(🗄️ Customer DB)]
    Appt --> DB4[(🗄️ Appointment DB)]
    Vacc --> DB5[(🗄️ Vaccination DB)]
    Inv --> DB6[(🗄️ Inventory DB)]
    Hosp --> DB7[(🗄️ Hospital DB)]
    File --> DB8[(🗄️ FileStorage DB)]
    
    Gateway --> Redis[🔴 Redis Cache]
    
    Appt --> RabbitMQ[🐰 RabbitMQ]
    Vacc --> RabbitMQ
    Inv --> RabbitMQ
    RabbitMQ --> Email[📧 Email Worker]
    
    subgraph "☁️ External Services"
        Cloud[☁️ Cloudinary]
        AWS[☁️ AWS S3]
    end
    
    File --> Cloud
    File --> AWS
    
    subgraph "📊 Monitoring"
        Seq[📊 Seq Logging]
    end
    
    Auth -.-> Seq
    Appt -.-> Seq
    Vacc -.-> Seq
    Inv -.-> Seq
```

</div>

### 🔧 Architecture Patterns
- **🎯 Microservices Architecture** - Individual services with dedicated databases
- **🚪 API Gateway** - YARP for request routing and load balancing  
- **⚡ Event-Driven** - RabbitMQ message broker for asynchronous communication
- **🚀 gRPC Communication** - High-performance inter-service communication
- **📋 CQRS Pattern** - Command Query Responsibility Segregation implementation
- **🐳 Docker Containers** - Fully containerized microservices

## 🧩 Microservices Architecture

<div align="center">

| Service | Type | Port | Description | Technologies |
|---------|------|------|-------------|--------------|
| 🚪 **API Gateway** | Gateway | 6060 | Request routing & load balancing | YARP, Rate Limiting |
| 🔐 **Authentication** | REST API | 6062 | User auth & JWT management | ASP.NET Identity, JWT |
| 👥 **HumanResource** | gRPC | 6061 | Employee, roles & departments | gRPC, PostgreSQL |
| 👤 **CustomerInfo** | gRPC | 6064 | Patient information | gRPC, PostgreSQL |
| 📅 **Appointment** | REST API | 6069 | Scheduling & notifications | Quartz.NET, MassTransit |
| 💉 **VaccinationReception** | REST API | 6065 | Vaccination processes | Carter, FluentValidation |
| 📦 **Inventory** | REST API | 6063 | Medicine & vaccine inventory | PostgreSQL, MassTransit |
| 🏥 **HospitalService** | REST API | 6067 | Hospital services & fees | PostgreSQL |
| 📁 **FileStorage** | REST API | 6068 | Document & image storage | Cloudinary, AWS S3 |
| ⚙️ **Management** | REST API | 6066 | Administrative operations | gRPC Clients |
| 📧 **Email Worker** | Background | - | Email notifications | MassTransit, RabbitMQ |

</div>

### 🔧 Shared Infrastructure

<div align="center">

| Component | Port | Purpose | Configuration |
|-----------|------|---------|---------------|
| 🗄️ **PostgreSQL** | 5432 | Primary database | Multiple databases per service |
| 🔴 **Redis** | 6379 | Caching layer | Password: `Mediflow@123` |
| 🐰 **RabbitMQ** | 5672/15672 | Message broker | User: `mediflow` |
| 📊 **Seq** | 5341/5342 | Centralized logging | Web UI + Ingestion |

</div>

## 🚀 Deployment Guide

### 🔧 Quick Start

<details>
<summary><b>🛠️ Development Environment</b></summary>

```bash
# Clone the repository
git clone https://github.com/ddnhuy/MediFlow_BE.git
cd MediFlow_BE

# Run with Docker Compose (Development)
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Check services status
docker-compose ps
```

**Available at:**
- 🚪 API Gateway: http://localhost:6060
- 📊 Seq Logging: http://localhost:5342
- 🐰 RabbitMQ Management: http://localhost:15672
</details>

<details>
<summary><b>🌟 Production Environment</b></summary>

```bash
# Run with Docker Compose (Production)
docker-compose -f docker-compose.yml -f docker-compose.release.yml up -d

# Monitor logs
docker-compose logs -f
```

**Features:**
- 🔒 SSL/TLS certificates with Let's Encrypt
- 🌍 Domain-based routing
- 📈 Production-optimized settings
</details>

### ☸️ Kubernetes Deployment

<div align="center">

**🎯 Production-Ready Kubernetes Deployment with Advanced Orchestration**

</div>

#### 💡 Why Kubernetes?

<table>
<tr>
<td width="50%">

**🔄 Scalability & Performance**
- 📈 **Auto-scaling**: HPA based on CPU/memory
- ⚖️ **Load Balancing**: Built-in service discovery
- 🚀 **High Performance**: Optimized resource allocation

**🛡️ Reliability & Security**
- 🔧 **Self-healing**: Automatic pod restart
- 🔒 **Security**: RBAC, Secrets, NetworkPolicies
- 💾 **Persistence**: StatefulSets with PVCs

</td>
<td width="50%">

**🔄 Operations & Management**
- 🚀 **Zero Downtime**: Rolling updates
- 📊 **Monitoring**: Built-in health checks
- 🔄 **Rollback**: Easy version management
- ⚙️ **Configuration**: ConfigMaps & Secrets

</td>
</tr>
</table>

#### 📋 Deployment Commands

```bash
# Deploy to Kubernetes using Helm
helm install mediflow ./helm --namespace mediflow --create-namespace

# Upgrade deployment with zero downtime
helm upgrade mediflow ./helm --wait --timeout=10m

# Scale specific services
kubectl scale deployment appointment-api --replicas=3 -n mediflow

# Monitor deployment status
kubectl get pods,svc,ingress -l app.kubernetes.io/instance=mediflow -n mediflow
```

#### 🎛️ Kubernetes Resources

<div align="center">

| Resource Type | Usage | Purpose |
|---------------|-------|---------|
| 🚀 **Deployments** | All microservices | Stateless application management |
| 🗃️ **StatefulSets** | PostgreSQL, Redis, RabbitMQ | Stateful services with persistent storage |
| 🌐 **Services** | All components | Internal service discovery & load balancing |
| 🔗 **Ingress** | External access | NGINX-based external traffic routing |
| ⚙️ **ConfigMaps** | Configuration | Environment-specific settings |
| 🔐 **Secrets** | Credentials | Secure storage for sensitive data |
| 💾 **PVCs** | Databases | Persistent data storage |

</div>

## 🔄 CI/CD Pipeline

<div align="center">

**⚡ Automated Testing, Building, and Deployment with GitHub Actions**

![CI/CD Pipeline](https://img.shields.io/badge/Pipeline-Automated-green?style=for-the-badge&logo=github-actions)

</div>

### 🔄 Workflow Overview

<table>
<tr>
<td width="50%">

#### 🧪 **Testing Environment** 
**Branch:** `develop`

```mermaid
graph TD
    A[💻 Push to develop] --> B[📢 Discord Notification]
    B --> C[🏗️ Build & Test]
    C --> D[✅ Unit Tests]
    D --> E[🚀 Deploy to VPS]
    E --> F[🔄 Docker Rebuild]
    F --> G[📢 Status Update]
```

</td>
<td width="50%">

#### 🌟 **Production Environment**
**Branch:** `main`

```mermaid
graph TD
    A[🎯 Push to main] --> B[📢 Discord Notification]
    B --> C[🏗️ Build & Test]
    C --> D[🧪 Full Test Suite]
    D --> E[🐳 Docker Build & Push]
    E --> F[☸️ K8s Deployment]
    F --> G[📢 Deployment Status]
```

</td>
</tr>
</table>

### 📋 Pipeline Details

<details>
<summary><b>🧪 Testing Environment Pipeline</b></summary>

| Stage | Description | Tools Used |
|-------|-------------|------------|
| 📢 **Notification** | Workflow started alert | Discord Webhook |
| 🏗️ **Build** | .NET 8.0 SDK setup & build | dotnet CLI |
| ✅ **Test** | Unit tests with coverage | test.runsettings |
| 🚀 **Deploy** | VPS deployment via SSH | Docker Compose |
| 🧹 **Cleanup** | Build cache cleanup | Docker system prune |
| 📢 **Status** | Success/failure notification | Discord Webhook |

**Features:**
- ⚡ Fast feedback loop for development
- 🔄 Automatic VPS deployment
- � Real-time notifications

</details>

<details>
<summary><b>🌟 Production Environment Pipeline</b></summary>

| Stage | Description | Tools Used |
|-------|-------------|------------|
| 📢 **Notification** | Production deployment alert | Discord Webhook |
| 🏗️ **Build** | .NET 8.0 SDK & solution build | dotnet CLI |
| 🧪 **Test** | Comprehensive test suite | NUnit, xUnit |
| 🐳 **Docker** | Multi-arch image build & push | Docker Hub |
| ☸️ **Deploy** | Kubernetes deployment | Helm Charts |
| 🔄 **Update** | Rolling update with health checks | kubectl |
| 📢 **Status** | Deployment completion status | Discord Webhook |

**Features:**
- 🎯 Zero-downtime deployments
- � Production-grade security
- 📈 Automatic scaling
- 🔄 Easy rollback capability

</details>

<details>
<summary><b>🔍 Code Quality Pipeline</b></summary>

| Stage | Description | Metrics |
|-------|-------------|---------|
| 🔧 **Setup** | JDK 17 & .NET 8.0 environment | Multi-runtime |
| 📊 **Analysis** | SonarQube Cloud scanning | Coverage, Security |
| 🛡️ **Security** | Vulnerability assessment | OWASP, CVE |
| 🧹 **Quality** | Code smell detection | Maintainability |
| 💡 **Debt** | Technical debt analysis | Complexity |
| ✅ **Gate** | Quality gate validation | Pass/Fail |

**Quality Standards:**
- 📈 **Coverage**: >80% code coverage
- 🛡️ **Security**: Zero high/critical vulnerabilities  
- 📊 **Maintainability**: A rating
- 🔄 **Reliability**: A rating

</details>

### 🛠️ CI/CD Features

<div align="center">

| Feature | Development | Production |
|---------|-------------|------------|
| 🤖 **Automation** | ✅ Full automation | ✅ Full automation |
| 🧪 **Testing** | ✅ Unit tests | ✅ Comprehensive suite |
| 🐳 **Containerization** | ✅ Docker Compose | ✅ Kubernetes ready |
| 📢 **Notifications** | ✅ Discord webhooks | ✅ Multi-channel alerts |
| 🔒 **Security** | ✅ Basic scanning | ✅ Full security audit |
| 📊 **Monitoring** | ✅ Basic metrics | ✅ Advanced observability |
| 🔄 **Rollback** | ✅ Git revert | ✅ Helm rollback |

</div>

---

## ⚙️ Configuration

<details>
<summary><b>🌍 Environment Variables (Production)</b></summary>

### SSL & Domain Configuration
```bash
DOMAIN=your-domain.com                    # Your domain name for SSL certificates
CERT_PATH=/path/to/cert.pem              # SSL certificate path
CERT_KEYPATH=/path/to/private.key        # SSL certificate key path
```

### File Storage Configuration
```bash
# Cloudinary Settings
CLOUDINARY_CLOUD_NAME=your-cloud-name
CLOUDINARY_API_KEY=your-api-key
CLOUDINARY_API_SECRET=your-api-secret

# AWS S3 Settings
AWS_BUCKET_NAME=your-s3-bucket
AWS_REGION=us-east-1
AWS_ACCESS_KEY=your-access-key
AWS_SECRET_KEY=your-secret-key
```

### Email Service Configuration
```bash
EMAIL_SERVER=smtp.gmail.com
EMAIL_PORT=587
EMAIL_SENDER_NAME=MediFlow System
EMAIL_SENDER_EMAIL=noreply@mediflow.com
EMAIL_SENDER_PASSWORD=your-app-password
```

</details>

<details>
<summary><b>🔗 Service Ports & Endpoints</b></summary>

<div align="center">

### 🎯 Core Services

| Service | HTTP Port | HTTPS Port | Health Check |
|---------|-----------|------------|--------------|
| 🚪 **API Gateway** | 6060 | - | `/health` |
| 🔐 **Authentication** | 6062 | - | `/health` |
| ⚙️ **Management** | 6066 | - | `/health` |
| 📦 **Inventory** | 6063 | - | `/health` |

### 🌐 gRPC Services

| Service | HTTP Port | HTTPS Port | gRPC Endpoint |
|---------|-----------|------------|---------------|
| 👥 **HumanResource** | 6061 | 8081 | `grpc://localhost:6061` |
| 👤 **CustomerInfo** | 6064 | 8081 | `grpc://localhost:6064` |

### 🏥 Medical Services

| Service | HTTP Port | HTTPS Port | Purpose |
|---------|-----------|------------|---------|
| 📅 **Appointment** | 6069 | 8081 | Appointment management |
| 💉 **VaccinationReception** | 6065 | 8081 | Vaccination processes |
| 🏥 **HospitalService** | 6067 | 8081 | Hospital operations |
| 📁 **FileStorage** | 6068 | 8081 | Document management |

### 🗄️ Infrastructure

| Component | Port | Management UI | Credentials |
|-----------|------|---------------|-------------|
| 🗄️ **PostgreSQL** | 5432 | - | postgres/postgres |
| 🔴 **Redis** | 6379 | - | Password: `Mediflow@123` |
| 🐰 **RabbitMQ** | 5672 | [15672](http://localhost:15672) | mediflow/Mediflow@123 |
| 📊 **Seq Logging** | 5341 | [5342](http://localhost:5342) | No authentication |

</div>

</details>

---

## 🤝 Contributing

<div align="center">

[![Contributors](https://img.shields.io/github/contributors/ddnhuy/MediFlow_BE?style=flat-square)](https://github.com/ddnhuy/MediFlow_BE/contributors)
[![Issues](https://img.shields.io/github/issues/ddnhuy/MediFlow_BE?style=flat-square)](https://github.com/ddnhuy/MediFlow_BE/issues)
[![Pull Requests](https://img.shields.io/github/issues-pr/ddnhuy/MediFlow_BE?style=flat-square)](https://github.com/ddnhuy/MediFlow_BE/pulls)

</div>

### 📋 Development Workflow

1. 🍴 **Fork** the repository
2. 🌿 **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. 💻 **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. 📤 **Push** to the branch (`git push origin feature/AmazingFeature`)
5. 🔄 **Open** a Pull Request

### 🧪 Running Tests

```bash
# Run all tests
dotnet test MediFlow.sln

# Run tests with coverage
dotnet test MediFlow.sln --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/AppointmentService/AppointmentService.UnitTests
```

---

<div align="center">

**Made with ❤️ by the MediFlow Team**

</div>
