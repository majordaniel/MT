def ansibleTemplating(){
    sh 'ls'
    sh 'pwd'
    sh 'touch hosts.ini && echo ${hosts_ini} | base64 -d > hosts.ini'
    sh 'ANSIBLE_HOST_KEY_CHECKING=false ansible-playbook --timeout=60 -i hosts.ini ansible/playbook.yaml'
}

pipeline{
    agent any

    environment {
        IMG_TAG="${sh(script: 'echo \$GIT_COMMIT | cut -c -7 | tr -d \'[:space:]\' ', returnStdout: true ) }.$BUILD_ID"
        APP_NAME="firstnote"
    }

    stages{
          //  stage('Inject production variables'){
            //    parallel{
             //       stage('master'){
              //          when {branch 'master'}
              //      steps{
              //          script{
              //          withCredentials([file(credentialsId: 'firstnote-prod', variable: "PIPELINE_ENV")]) {load "$PIPELINE_ENV"}
              //          sh 'touch FirstNote.Api/appsettings.json && echo ${firstnote} | base64 -d > FirstNote.Api/appsettings.json'
              //          }
              //      }
              //      }

                //stage('dev'){
                //    when{
                //        not{
                //            anyOf{
                //                branch 'master'
                //                }
                //            }
                //        }
                //    steps{
                //        script{
                //        withCredentials([file(credentialsId: 'dev-k8s-cluster', variable: "PIPELINE_ENV")]) {load "$PIPELINE_ENV"}
                //        }
                //    }
                //}
                
                //}
            //}
            
            stage('build and push docker images'){
                parallel{
                    stage('build and push dev images'){
                        when{
                            not{
                                anyOf{
                                    branch 'master'
                                }
                            }
                        }
                        steps{
                            script{
                                firstnoteIMage = docker.build("registry-1.docker.io/fbnquestdocker/$APP_NAME-dev:$IMG_TAG" ,"-f FirstNote.Api/Dockerfile .")

                                docker.withRegistry('https://registry-1.docker.io/v2/','docker-registry'){
                                    firstnoteIMage.push()
                                }
                            }
                        }
                    }
                    stage('build and push prod images'){
                        when{branch 'master'}
                        steps{
                            script{
                                firstnoteIMage = docker.build("registry-1.docker.io/fbnquestdocker/$APP_NAME:$IMG_TAG" ,"-f FirstNote.Api/Dockerfile .")

                                docker.withRegistry('https://registry-1.docker.io/v2/','docker-registry'){
                                    firstnoteIMage.push()
                                } 
                            }
                        }
                    }
                }
            }
        

        stage('Deploy to k8s'){
            steps {
                script{
                    switch(BRANCH_NAME){
                        case["master"]:
                            env.domain_name= "fbnq-mspx-l01.fbnmerchantbank.com"
                            env.IMAGE_NAME="firstnote"
                            withKubeConfig([credentialsId: 'prod-kube-config', serverUrl: 'https://172.17.0.219:6443']) {
                            sh 'kubectl get deployments'
                            sh 'ls'
                            sh 'pwd'
                            ansibleTemplating()
                            sh 'kubectl apply -f deployments.yaml'
                            sh 'kubectl apply -f ingress.yaml'
                            sh 'kubectl apply -f services.yaml'
                            }
                            break

                        default:
                            env.domain_name= "k8s.fbnmerchantbank.com"
                            env.IMAGE_NAME="firstnote-dev"

                            withKubeConfig([credentialsId: 'kube-config', serverUrl: 'https://10.100.33.124:6443']) {
                            sh 'kubectl get deployments'
                            ansibleTemplating()
                            sh 'ls'
                            sh 'pwd'
                            sh 'kubectl apply -f deployments.yaml'
                            sh 'kubectl apply -f ingress.yaml'
                            sh 'kubectl apply -f services.yaml'
                            }
                            break
                    }
                }                    
            }
        }
    }
}
