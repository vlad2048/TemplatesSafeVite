# Create a Solid project
```ps1
mkdir solider
cd solider
```

## Create ./package.json
```json
{
  "name": "solider",
  "version": "1.0.0",
  "description": "",
  "license": "MIT",
  "scripts": {
    "dev": "vite --host"
  }
}
```

## Add dependencies
```ps1
yarn add -D typescript
yarn add -D vite
yarn add -D vite-plugin-solid
yarn add solid-js
```

## Create ./tsconfig.json
```js
{
  "compilerOptions": {
    "target": "ESNext",
    "module": "ESNext",
    "moduleResolution": "node",
    "allowSyntheticDefaultImports": true,
    "esModuleInterop": true,
    "jsx": "preserve",
    "jsxImportSource": "solid-js",
    "types": ["vite/client"],
    "noEmit": true,
    "isolatedModules": true
  }
}
```

## Create ./vite.config.ts
```js
import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';

export default defineConfig({
  plugins: [solidPlugin()],
  server: {
    port: 3000,
  },
  build: {
    target: 'esnext',
  },
});
```

## Create ./index.html
```html
<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="theme-color" content="#000000" />
    <title>Solid App</title>
  </head>
  <body>
    <noscript>You need to enable JavaScript to run this app.</noscript>
    <div id="root"></div>

    <script src="/src/index.tsx" type="module"></script>
  </body>
</html>
```

## Add source files
### ./src/index.tsx
```tsx
/* @refresh reload */
import { render } from 'solid-js/web';
import { App } from './App';

render(() => <App />, document.getElementById('root') as HTMLElement);
```

### ./src/App.tsx
```tsx
import type { Component } from 'solid-js';
import { Comp } from './Comp';

export const App: Component = () => {
  return (
    <>
      <h1>Hello world!!!!</h1>
      <Comp />
    </>
  );
};
```

### ./src/Comp.tsx
```tsx
export const Comp = () => {
  return <h1>Hello world!!!</h1>;
};
```

## Run
```ps1
yarn
yarn start
```



# Use Solid

## Basics
```js
export const App: Component = () => {
    // signal
    const [cnt, setCnt] = createSignal(0);
    const [num, setNum] = createSignal(10);

    // derived signal
    const doubleNum = () => num() * 2;

    setInterval(() => setCnt(e => e + 1), 120);

    // run when cnt() changes / after rendering is finished
    createEffect(() => console.log(`cnt=${cnt()}`));
    // use createRenderEffect to run before rendering is finished

    const fib = createMemo(() => fibonacci(count()));

    return (
        <>
            <h1>cnt = {cnt()}</h1>
            <h1>num = {num()}</h1>
            <h1>nu2 = {doubleNum()}</h1>
            <Comp />
            <button onClick={() => setNum(e => e + 1)}>Click Me</button>
        </>
    );
};
```

## Control Flow
```js
<Show
    when={loggedIn()}
    fallback={<button onClick={toggle}>Log in</button>}
>
    <button onClick={toggle}>Log out</button>
</Show>
```

### \<For>: objects can change position (index)
```js
<For each={cats()}>
    { (cat, i) =>
        <li>
            <a target="_blank" href={`https://www.youtube.com/watch?v=${cat.id}`}>
                {i() + 1}: {cat.name}
            </a>
        </li>
    }
</For>
```

### \<Index>: objects can change value in the same position (index)
Use \<Index> when working with primitive values
```js
<Index each={cats()}>
    { (cat, i) =>
        <li>
            <a target="_blank" href={`https://www.youtube.com/watch?v=${cat().id}`}>
                {i + 1}: {cat().name}
            </a>
        </li>
    }
</Index>
```

```js
<Switch fallback={<p>{x()} is between 5 and 10</p>}>
    <Match when={x() > 10}>
        <p>{x()} is greater than 10</p>
    </Match>
    <Match when={5 > x()}>
        <p>{x()} is less than 5</p>
    </Match>
</Switch>
```

```js
const comp = () => <strong style="color: red">Red Thing</strong>;
<Dynamic component={comp} />
```

```js
// inserted in a <div> in document.body by default
<Portal>
...
</Portal>
```

```js
<ErrorBoundary fallback={err => err}>
    <Broken />
</ErrorBoundary>
```

## Lifecycle
```js
// Similar to createEffect() but
//   - without tracking
//   - runs only once (after initial rendering)
onMount(async () => ...);

// Called when the scope it's in:
//  - reevaluates
//  - is disposed
onCleanup(() => clearInterval(timer));

// Events
<div onMouseMove={handleMouseMove}>
  The mouse position is {pos().x} x {pos().y}
</div>
```

## Styles
```js
<div style={{
    color: `rgb(${num()}, 180, ${num()})`,
    "font-weight": 800,
    "font-size": `${num()}px`}}
>
    Some Text
</div>

<button
  classList={{selected: current() === 'foo'}}
  onClick={() => setCurrent('foo')}
>
    foo
</button>

import { active } from "./style.module.css"
<div classList={{ [active]: isActive() }} />
```

## Directives
```js
<div class="modal" use:clickOutside={() => setShow(false)}>
    Some Modal
</div>

// el:          DOM element using the directive
// accessor:    getter for the parameter
export default function clickOutside(el, accessor) {
    const onClick = (e) => !el.contains(e.target) && accessor()?.();
    document.body.addEventListener("click", onClick);
    onCleanup(() => document.body.removeEventListener("click", onClick));
}
```

## Stores
```js
import { createSignal, createContext, useContext } from "solid-js";

const CounterContext = createContext();

export function CounterProvider(props) {
    const [count, setCount] = createSignal(props.count || 0),
    counter = [
        count,
        {
            increment() {
                setCount(c => c + 1);
            },
            decrement() {
                setCount(c => c - 1);
            }
        }
    ];

    return (
        <CounterContext.Provider value={counter}>
        {props.children}
        </CounterContext.Provider>
    );
}

export function useCounter() {
    return useContext(CounterContext);
}
```