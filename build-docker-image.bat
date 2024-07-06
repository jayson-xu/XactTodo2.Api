call docker build -t 10.1.8.40:1080/xzc/xacttodoapi -f .\XactTodo.Api\Dockerfile .
docker login 10.1.8.40:1080 -u xzc
docker push 10.1.8.40:1080/xzc/xacttodoapi
