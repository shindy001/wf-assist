const idChars =
  "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
const base = idChars.length; // 62

function toBase62(num: number) {
  let str = "";
  do {
    str = idChars[num % base] + str;
    num = Math.floor(num / base);
  } while (num > 0);
  return str.padStart(3, "0");
}

function fromBase62(str: string) {
  let num = 0;
  for (let char of str) {
    num = num * base + idChars.indexOf(char);
  }
  return num;
}

export function getNextId(lastId: string = "000") {
  let lastIdCounter = fromBase62(lastId);
  lastIdCounter++;
  return toBase62(lastIdCounter);
}
