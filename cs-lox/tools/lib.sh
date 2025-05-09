# @type { (filePath: string, data: string) => void }
function appendLine {
  filePath=$1
  data=$2

  echo "${data}" >> "${filePath}"
}

# @type { (filePath: string) => void }
function clearFile {
  filePath=$1

  truncate -s 0 $filePath
}

# @type { (source: string) => string }
function trim {
  local source=$1
  local trimmedSource=$(echo "${source}" | xargs)

  echo "${trimmedSource}"
}

# TODO: think how to return an array from function
function splitBy {
  local delimiter=$1
  local string=$2
  local -n outArray=$3

  IFS=$delimiter read -ra outArray <<< "$string"
}
