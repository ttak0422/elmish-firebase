import { read, signedIn } from "./Main.fs";

const firebase = require("firebase");
// Required for side-effects
require("firebase/firestore");

// Initialize Cloud Firestore through Firebase
const firebaseConfig = {
    apiKey: "AIzaSyBEGZsx024y1QqIWf4obDE9qiAvG1lKKmk",
    authDomain: "elmish-firebase.firebaseapp.com",
    databaseURL: "https://elmish-firebase.firebaseio.com",
    projectId: "elmish-firebase",
    storageBucket: "elmish-firebase.appspot.com",
    messagingSenderId: "147064605223",
    appId: "1:147064605223:web:29c1bba5be8f78b9d51c02"
};

// Initialize Firebase
firebase.initializeApp(firebaseConfig);
const provider = new firebase.auth.GoogleAuthProvider();
const db = firebase.firestore();

firebase.auth().onAuthStateChanged((user) => {
    if (user) {
        signedIn(true);
        db.collection("foo").doc(user.uid).onSnapshot((doc) => {
            const data = doc.data();
            read(data.input);
        });
    }
})

function signIn() {
    firebase.auth()
        .signInWithPopup(provider)
        .then((_) => { })
        .catch((error) => { console.error(error) });
}

function push(text) {
    const user = firebase.auth().currentUser;
    db.collection("foo").doc(user.uid).set({ input: text });
}

// Call JS from F#
export { signIn, push }
