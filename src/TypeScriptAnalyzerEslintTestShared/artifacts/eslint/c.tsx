import * as React from 'react';
import * as ReactDOM from 'react-dom';

export class MyTsx {
    sayHello() {
        ReactDOM.render(
            <h1> Hello tsx World! </h1>,
            document.getElementById('wrapper')
        );
    }
    getX(): number {
        var x = 3.141;
        return x;
    }

}

const inst = ReactDOM.render(<App />, document.body);
