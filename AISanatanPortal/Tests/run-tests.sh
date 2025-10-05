#!/bin/bash

# Bash script to run all tests for the AI Sanatan Portal

# Default values
CONFIGURATION="Debug"
COVERAGE=false
VERBOSE=false
FILTER=""
HELP=false

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Function to show help
show_help() {
    echo -e "${GREEN}AI Sanatan Portal Test Runner${NC}"
    echo -e "${GREEN}=============================${NC}"
    echo ""
    echo -e "${YELLOW}Usage: ./run-tests.sh [options]${NC}"
    echo ""
    echo -e "${YELLOW}Options:${NC}"
    echo -e "${WHITE}  -c, --configuration <config>  Build configuration (Debug|Release) [Default: Debug]${NC}"
    echo -e "${WHITE}  --coverage                   Generate code coverage report${NC}"
    echo -e "${WHITE}  -v, --verbose                Enable verbose output${NC}"
    echo -e "${WHITE}  -f, --filter <filter>        Filter tests by name or category${NC}"
    echo -e "${WHITE}  -h, --help                   Show this help message${NC}"
    echo ""
    echo -e "${YELLOW}Examples:${NC}"
    echo -e "${WHITE}  ./run-tests.sh                          # Run all tests${NC}"
    echo -e "${WHITE}  ./run-tests.sh --coverage               # Run with coverage${NC}"
    echo -e "${WHITE}  ./run-tests.sh --filter 'AIDataAgent'   # Run specific tests${NC}"
    echo -e "${WHITE}  ./run-tests.sh --verbose --coverage     # Verbose with coverage${NC}"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        --coverage)
            COVERAGE=true
            shift
            ;;
        -v|--verbose)
            VERBOSE=true
            shift
            ;;
        -f|--filter)
            FILTER="$2"
            shift 2
            ;;
        -h|--help)
            HELP=true
            shift
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            show_help
            exit 1
            ;;
    esac
done

if [ "$HELP" = true ]; then
    show_help
    exit 0
fi

echo -e "${GREEN}AI Sanatan Portal Test Runner${NC}"
echo -e "${GREEN}=============================${NC}"
echo ""

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: .NET SDK not found. Please install .NET 9 SDK.${NC}"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo -e "${CYAN}Using .NET version: $DOTNET_VERSION${NC}"

# Build the project first
echo -e "${YELLOW}Building project...${NC}"
BUILD_ARGS=("build" "--configuration" "$CONFIGURATION")

if [ "$VERBOSE" = true ]; then
    BUILD_ARGS+=("--verbosity" "detailed")
fi

if ! dotnet "${BUILD_ARGS[@]}"; then
    echo -e "${RED}Build failed${NC}"
    exit 1
fi

echo -e "${GREEN}Build completed successfully.${NC}"

# Prepare test arguments
TEST_ARGS=("test" "--configuration" "$CONFIGURATION" "--no-build")

if [ "$VERBOSE" = true ]; then
    TEST_ARGS+=("--verbosity" "detailed")
    TEST_ARGS+=("--logger" "console;verbosity=detailed")
fi

if [ "$COVERAGE" = true ]; then
    TEST_ARGS+=("--collect" "XPlat Code Coverage")
fi

if [ -n "$FILTER" ]; then
    TEST_ARGS+=("--filter" "$FILTER")
fi

# Add test logger for better output
TEST_ARGS+=("--logger" "trx;LogFileName=test-results.trx")

echo -e "${YELLOW}Running tests...${NC}"
echo -e "${GRAY}Command: dotnet ${TEST_ARGS[*]}${NC}"

if dotnet "${TEST_ARGS[@]}"; then
    TEST_EXIT_CODE=0
    echo -e "${GREEN}All tests passed!${NC}"
else
    TEST_EXIT_CODE=$?
    echo -e "${RED}Some tests failed. Exit code: $TEST_EXIT_CODE${NC}"
fi

# Generate coverage report if requested
if [ "$COVERAGE" = true ]; then
    echo -e "${YELLOW}Generating coverage report...${NC}"
    
    # Check if reportgenerator is installed
    if ! dotnet tool list -g | grep -q "reportgenerator"; then
        echo -e "${YELLOW}Installing reportgenerator tool...${NC}"
        dotnet tool install -g dotnet-reportgenerator-globaltool
    fi
    
    # Find coverage files
    COVERAGE_FILE=$(find . -name "coverage.cobertura.xml" -type f | head -1)
    if [ -n "$COVERAGE_FILE" ]; then
        REPORT_DIR="./TestResults/CoverageReport"
        
        if reportgenerator -reports:"$COVERAGE_FILE" -targetdir:"$REPORT_DIR" -reporttypes:"Html"; then
            echo -e "${GREEN}Coverage report generated at: $REPORT_DIR/index.html${NC}"
            
            # Try to open coverage report in browser (Linux/macOS)
            if command -v xdg-open &> /dev/null; then
                xdg-open "$REPORT_DIR/index.html" 2>/dev/null &
            elif command -v open &> /dev/null; then
                open "$REPORT_DIR/index.html" 2>/dev/null &
            else
                echo -e "${YELLOW}Could not open coverage report automatically.${NC}"
            fi
        else
            echo -e "${RED}Failed to generate coverage report.${NC}"
        fi
    else
        echo -e "${YELLOW}No coverage files found.${NC}"
    fi
fi

# Show test results summary
TRX_FILE=$(find . -name "test-results.trx" -type f | head -1)
if [ -n "$TRX_FILE" ]; then
    echo -e "${CYAN}Test results saved to: $TRX_FILE${NC}"
fi

echo ""
echo -e "${GREEN}Test run completed.${NC}"

exit $TEST_EXIT_CODE


