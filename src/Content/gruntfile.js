sass: {
    dist: {
        options: {
            style: 'compressed',
            sourcemap: 'none',
            noCache: true
        },
        files: {
            '../css/default.min.css': '../css/default.scss',
        }
    }
}, // end sass