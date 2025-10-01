#!/bin/bash

echo "🕉️  AI Sanatan Portal - Frontend Installation"
echo "============================================="

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed. Please install Node.js 18+ first."
    exit 1
fi

# Check Node.js version
NODE_VERSION=$(node -v | cut -d'v' -f2 | cut -d'.' -f1)
if [ "$NODE_VERSION" -lt "18" ]; then
    echo "❌ Node.js version $NODE_VERSION detected. Please upgrade to Node.js 18 or later."
    exit 1
fi

echo "✅ Node.js $(node -v) detected"

# Check if npm is installed
if ! command -v npm &> /dev/null; then
    echo "❌ npm is not installed. Please install npm first."
    exit 1
fi

echo "✅ npm $(npm -v) detected"

# Install dependencies
echo "📦 Installing npm dependencies..."
npm install

if [ $? -eq 0 ]; then
    echo "✅ Dependencies installed successfully!"
    echo ""
    echo "🚀 You can now start the development server with:"
    echo "   npm start"
    echo "   or"
    echo "   ng serve"
    echo ""
    echo "📖 The application will be available at: http://localhost:4200"
    echo ""
    echo "🕉️  May your development journey be blessed!"
else
    echo "❌ Failed to install dependencies. Please check the error messages above."
    exit 1
fi