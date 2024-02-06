type GreetingLike = string | (() => string);
const a: GreetingLike = "";
console.log(a);

// Generates typescript-eslint error as is unused
type Unused = string | (() => string);
// Generates eslint error
var x = 1;