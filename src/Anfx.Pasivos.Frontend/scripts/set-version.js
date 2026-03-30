const fs = require('fs');
const p  = require('../package.json');
const files = [
  'src/environments/environment.ts',
  'src/environments/environment.production.ts',
];
files.forEach(f => {
  const content = fs.readFileSync(f, 'utf8');
  fs.writeFileSync(f, content.replace(/appVersion: '.*?'/, `appVersion: '${p.version}'`));
  console.log(`Version ${p.version} → ${f}`);
});
