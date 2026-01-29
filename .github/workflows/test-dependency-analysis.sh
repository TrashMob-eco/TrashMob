#!/bin/bash
# Test script to validate dependency analysis workflow logic
# This simulates what the GitHub Actions workflow does

set -e

echo "🧪 Testing Dependency Analysis Workflow Logic"
echo "=============================================="
echo ""

# Create temporary test directories
TEST_DIR="/tmp/dependency-analysis-test"
rm -rf "$TEST_DIR"
mkdir -p "$TEST_DIR"

# Test 1: NPM Major Version Change
echo "Test 1: NPM Major Version Change"
echo "---------------------------------"

# Create base package.json
cat > "$TEST_DIR/base_package.json" << 'EOF'
{
  "dependencies": {
    "react": "^18.0.0",
    "axios": "^1.6.0"
  }
}
EOF

# Create updated package.json with major version change
cat > "$TEST_DIR/current_package.json" << 'EOF'
{
  "dependencies": {
    "react": "^19.0.0",
    "axios": "^1.6.2"
  }
}
EOF

# Run analysis
node << 'NODEJS'
const fs = require('fs');
const basePackage = JSON.parse(fs.readFileSync('/tmp/dependency-analysis-test/base_package.json', 'utf8'));
const currentPackage = JSON.parse(fs.readFileSync('/tmp/dependency-analysis-test/current_package.json', 'utf8'));

const baseDeps = { ...basePackage.dependencies, ...basePackage.devDependencies };
const currentDeps = { ...currentPackage.dependencies, ...currentPackage.devDependencies };

let hasMajorChanges = false;

console.log("Detected changes:");
Object.keys(currentDeps).forEach(pkg => {
  if (!baseDeps[pkg]) {
    console.log('- NEW: `' + pkg + '` @ `' + currentDeps[pkg] + '`');
  } else if (baseDeps[pkg] !== currentDeps[pkg]) {
    const oldVer = baseDeps[pkg].replace(/[\^~>=<]/, '');
    const newVer = currentDeps[pkg].replace(/[\^~>=<]/, '');
    
    const oldMajor = oldVer.split('.')[0];
    const newMajor = newVer.split('.')[0];
    
    if (oldMajor !== newMajor && newMajor !== '0') {
      console.log('- ⚠️ MAJOR UPDATE: `' + pkg + '` from `' + baseDeps[pkg] + '` to `' + currentDeps[pkg] + '`');
      hasMajorChanges = true;
    } else {
      console.log('- ✅ UPDATED: `' + pkg + '` from `' + baseDeps[pkg] + '` to `' + currentDeps[pkg] + '`');
    }
  }
});

if (hasMajorChanges) {
  console.log("\n✅ PASS: Major version change detected correctly");
  process.exit(0);
} else {
  console.log("\n❌ FAIL: Major version change NOT detected");
  process.exit(1);
}
NODEJS

echo ""

# Test 2: NPM Minor Version Change
echo "Test 2: NPM Minor Version Change (No Breaking Changes)"
echo "-------------------------------------------------------"

# Create updated package.json with only minor version changes
cat > "$TEST_DIR/current_package.json" << 'EOF'
{
  "dependencies": {
    "react": "^18.2.0",
    "axios": "^1.7.0"
  }
}
EOF

# Run analysis
node << 'NODEJS'
const fs = require('fs');
const basePackage = JSON.parse(fs.readFileSync('/tmp/dependency-analysis-test/base_package.json', 'utf8'));
const currentPackage = JSON.parse(fs.readFileSync('/tmp/dependency-analysis-test/current_package.json', 'utf8'));

const baseDeps = { ...basePackage.dependencies, ...basePackage.devDependencies };
const currentDeps = { ...currentPackage.dependencies, ...currentPackage.devDependencies };

let hasMajorChanges = false;

console.log("Detected changes:");
Object.keys(currentDeps).forEach(pkg => {
  if (!baseDeps[pkg]) {
    console.log('- NEW: `' + pkg + '` @ `' + currentDeps[pkg] + '`');
  } else if (baseDeps[pkg] !== currentDeps[pkg]) {
    const oldVer = baseDeps[pkg].replace(/[\^~>=<]/, '');
    const newVer = currentDeps[pkg].replace(/[\^~>=<]/, '');
    
    const oldMajor = oldVer.split('.')[0];
    const newMajor = newVer.split('.')[0];
    
    if (oldMajor !== newMajor && newMajor !== '0') {
      console.log('- ⚠️ MAJOR UPDATE: `' + pkg + '` from `' + baseDeps[pkg] + '` to `' + currentDeps[pkg] + '`');
      hasMajorChanges = true;
    } else {
      console.log('- ✅ UPDATED: `' + pkg + '` from `' + baseDeps[pkg] + '` to `' + currentDeps[pkg] + '`');
    }
  }
});

if (!hasMajorChanges) {
  console.log("\n✅ PASS: No major version changes (correctly identified as safe)");
  process.exit(0);
} else {
  console.log("\n❌ FAIL: False positive for major version change");
  process.exit(1);
}
NODEJS

echo ""

# Test 3: NuGet Package Changes
echo "Test 3: NuGet Package Analysis"
echo "-------------------------------"

# Create base .csproj
cat > "$TEST_DIR/base.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.1" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
</Project>
EOF

# Create updated .csproj with major version change
cat > "$TEST_DIR/current.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="11.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.1" />
  </ItemGroup>
</Project>
EOF

echo "Analyzing NuGet package changes..."
base_packages=$(grep -oP '<PackageReference Include="\K[^"]+' "$TEST_DIR/base.csproj" || echo "")
current_packages=$(grep -oP '<PackageReference Include="\K[^"]+' "$TEST_DIR/current.csproj" || echo "")

has_major=false

while IFS= read -r pkg; do
  if [ -n "$pkg" ]; then
    base_ver=$(grep "PackageReference Include=\"$pkg\"" "$TEST_DIR/base.csproj" | grep -oP 'Version="\K[^"]+' || echo "")
    current_ver=$(grep "PackageReference Include=\"$pkg\"" "$TEST_DIR/current.csproj" | grep -oP 'Version="\K[^"]+' || echo "")

    if [ -z "$base_ver" ] && [ -n "$current_ver" ]; then
      echo "- NEW: $pkg @ $current_ver"
    elif [ "$base_ver" != "$current_ver" ] && [ -n "$current_ver" ]; then
      base_major=$(echo "$base_ver" | cut -d. -f1)
      current_major=$(echo "$current_ver" | cut -d. -f1)

      if [ "$base_major" != "$current_major" ]; then
        echo "- ⚠️ MAJOR UPDATE: $pkg from $base_ver to $current_ver"
        has_major=true
      else
        echo "- ✅ UPDATED: $pkg from $base_ver to $current_ver"
      fi
    fi
  fi
done <<< "$current_packages"

if [ "$has_major" = true ]; then
  echo ""
  echo "✅ PASS: NuGet major version change detected correctly"
else
  echo ""
  echo "❌ FAIL: NuGet major version change NOT detected"
  exit 1
fi

echo ""
echo "=============================================="
echo "✅ All tests passed!"
echo ""
echo "The dependency analysis logic works correctly for:"
echo "  ✓ NPM major version changes (breaking)"
echo "  ✓ NPM minor/patch changes (safe)"
echo "  ✓ NuGet major version changes (breaking)"
