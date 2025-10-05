# PowerShell script to run all tests for the AI Sanatan Portal

param(
    [string]$Configuration = "Debug",
    [switch]$Coverage = $false,
    [switch]$Verbose = $false,
    [string]$Filter = "",
    [switch]$Help = $false
)

if ($Help) {
    Write-Host "AI Sanatan Portal Test Runner" -ForegroundColor Green
    Write-Host "=============================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Usage: .\run-tests.ps1 [options]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Options:" -ForegroundColor Yellow
    Write-Host "  -Configuration <config>  Build configuration (Debug|Release) [Default: Debug]" -ForegroundColor White
    Write-Host "  -Coverage               Generate code coverage report" -ForegroundColor White
    Write-Host "  -Verbose                Enable verbose output" -ForegroundColor White
    Write-Host "  -Filter <filter>        Filter tests by name or category" -ForegroundColor White
    Write-Host "  -Help                   Show this help message" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Yellow
    Write-Host "  .\run-tests.ps1                          # Run all tests" -ForegroundColor White
    Write-Host "  .\run-tests.ps1 -Coverage                # Run with coverage" -ForegroundColor White
    Write-Host "  .\run-tests.ps1 -Filter \"AIDataAgent\"    # Run specific tests" -ForegroundColor White
    Write-Host "  .\run-tests.ps1 -Verbose -Coverage       # Verbose with coverage" -ForegroundColor White
    exit 0
}

Write-Host "AI Sanatan Portal Test Runner" -ForegroundColor Green
Write-Host "=============================" -ForegroundColor Green
Write-Host ""

# Set error action preference
$ErrorActionPreference = "Stop"

# Check if dotnet is available
try {
    $dotnetVersion = dotnet --version
    Write-Host "Using .NET version: $dotnetVersion" -ForegroundColor Cyan
} catch {
    Write-Host "Error: .NET SDK not found. Please install .NET 9 SDK." -ForegroundColor Red
    exit 1
}

# Build the project first
Write-Host "Building project..." -ForegroundColor Yellow
try {
    $buildArgs = @("build", "--configuration", $Configuration)
    if ($Verbose) { $buildArgs += "--verbosity", "detailed" }
    
    dotnet $buildArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }
    Write-Host "Build completed successfully." -ForegroundColor Green
} catch {
    Write-Host "Build failed: $_" -ForegroundColor Red
    exit 1
}

# Prepare test arguments
$testArgs = @("test", "--configuration", $Configuration, "--no-build")

if ($Verbose) {
    $testArgs += "--verbosity", "detailed"
    $testArgs += "--logger", "console;verbosity=detailed"
}

if ($Coverage) {
    $testArgs += "--collect", "XPlat Code Coverage"
}

if ($Filter -ne "") {
    $testArgs += "--filter", $Filter
}

# Add test logger for better output
$testArgs += "--logger", "trx;LogFileName=test-results.trx"

Write-Host "Running tests..." -ForegroundColor Yellow
Write-Host "Command: dotnet $($testArgs -join ' ')" -ForegroundColor Gray

try {
    dotnet $testArgs
    $testExitCode = $LASTEXITCODE
    
    if ($testExitCode -eq 0) {
        Write-Host "All tests passed!" -ForegroundColor Green
    } else {
        Write-Host "Some tests failed. Exit code: $testExitCode" -ForegroundColor Red
    }
    
    # Generate coverage report if requested
    if ($Coverage) {
        Write-Host "Generating coverage report..." -ForegroundColor Yellow
        
        # Check if reportgenerator is installed
        try {
            dotnet tool list -g | Select-String "reportgenerator" | Out-Null
            if ($LASTEXITCODE -ne 0) {
                Write-Host "Installing reportgenerator tool..." -ForegroundColor Yellow
                dotnet tool install -g dotnet-reportgenerator-globaltool
            }
            
            # Find coverage files
            $coverageFiles = Get-ChildItem -Path "." -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1
            if ($coverageFiles) {
                $coverageFile = $coverageFiles.FullName
                $reportDir = ".\TestResults\CoverageReport"
                
                reportgenerator -reports:$coverageFile -targetdir:$reportDir -reporttypes:"Html"
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "Coverage report generated at: $reportDir\index.html" -ForegroundColor Green
                    
                    # Open coverage report in browser
                    try {
                        Start-Process "$reportDir\index.html"
                    } catch {
                        Write-Host "Could not open coverage report automatically." -ForegroundColor Yellow
                    }
                } else {
                    Write-Host "Failed to generate coverage report." -ForegroundColor Red
                }
            } else {
                Write-Host "No coverage files found." -ForegroundColor Yellow
            }
        } catch {
            Write-Host "Error generating coverage report: $_" -ForegroundColor Red
        }
    }
    
    # Show test results summary
    $trxFiles = Get-ChildItem -Path "." -Recurse -Filter "test-results.trx" | Select-Object -First 1
    if ($trxFiles) {
        Write-Host "Test results saved to: $($trxFiles.FullName)" -ForegroundColor Cyan
    }
    
    Write-Host ""
    Write-Host "Test run completed." -ForegroundColor Green
    
    exit $testExitCode
    
} catch {
    Write-Host "Error running tests: $_" -ForegroundColor Red
    exit 1
}


