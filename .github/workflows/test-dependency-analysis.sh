#!/bin/bash
# Test script to validate dependency analysis workflow logic

set -e

echo "🧪 Testing Dependency Analysis Workflow Logic"
echo "=============================================="
echo ""

TEST_DIR="/tmp/dependency-analysis-test"
rm -rf "$TEST_DIR"
mkdir -p "$TEST_DIR"

# Test 1: NPM Major Version Change
echo "Test 1: NPM Major Version Change"
echo "---------------------------------"

cat > "$TEST_DIR/base_package.json" << 'EOF'
{"dependencies": {"react": "^18.0.0", "axios": "^1.6.0"}}
EOF

cat > "$TEST_DIR/current_package.json" << 'EOF'
{"dependencies": {"react": "^19.0.0", "axios": "^1.6.2"}}
EOF

node << 'NODEJS'
const fs = require('fs');
const basePackage = JSON.parse(fs.readFileSync('/tmp/dependency-analysis-test/base_package.json', 'utf8'));
const currentPackage = JSON.parse(fs.readFileSync('/tmp/dependency-analysis-test/current_package.json', 'utf8'));
const baseDeps = { ...basePackage.dependencies };
const currentDeps = { ...currentPackage.dependencies };
let hasMajorChanges = false;
console.log("Detected changes:");
Object.keys(currentDeps).forEach(pkg => {
  if (baseDeps[pkg] && baseDeps[pkg] !== currentDeps[pkg]) {
    const oldVer = baseDeps[pkg].replace(/^[\^~>=<]+/, '');
    const newVer = currentDeps[pkg].replace(/^[\^~>=<]+/, '');
    const oldMajor = parseInt(oldVer.split('.')[0], 10);
    const newMajor = parseInt(newVer.split('.')[0], 10);
    const oldMinor = parseInt(oldVer.split('.')[1], 10) || 0;
    const newMinor = parseInt(newVer.split('.')[1], 10) || 0;
    if ((oldMajor !== newMajor && newMajor !== 0) || (oldMajor === 0 && newMajor === 0 && oldMinor !== newMinor)) {
      console.log('- ⚠️ MAJOR UPDATE: `' + pkg + '` from `' + baseDeps[pkg] + '` to `' + currentDeps[pkg] + '`');
      hasMajorChanges = true;
    } else {
      console.log('- ✅ UPDATED: `' + pkg + '` from `' + baseDeps[pkg] + '` to `' + currentDeps[pkg] + '`');
    }
  }
});
if (hasMajorChanges) {
  console.log("\n✅ PASS: Major version change detected");
  process.exit(0);
} else {
  console.log("\n❌ FAIL: Major version change NOT detected");
  process.exit(1);
}
NODEJS

echo ""
echo "=============================================="
echo "✅ All tests passed!"
