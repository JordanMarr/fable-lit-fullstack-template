# Lit.TodoMVC

Sample [Fable.Lit](https://github.com/fable-compiler/Fable.Lit) app. To start a development server run:

```
npm install && npm start
```

Other commands:

```bash
npm test                        # Run tests
npm run test:watch              # Run tests in watch mode
npm test -- --update-snapshots  # Update test snapshots
npm run build   # Build optimized site for deployment and put in dist/
```

## Vite.js repository structure conventions

- Put static files in `public/` folder
- Put `index.html` in app root (next to `package.json`)
- Add a reference to the entry JS file (relative path is important):

```html
<script type="module" src="./build/client/App.js"></script>
```
