variable "region" {
  type    = string
  default = "us-east-1"
}

variable "project_name" {
  type    = string
  default = "canais25"
}

variable "container_port" {
  type    = number
  default = 3000
}

variable "desired_count" {
  type    = number
  default = 2
}


variable "environment" {
  type        = string
  description = "Ambiente da aplicação"
}

variable "cpu" {
  type        = number
  description = "CPU da task"
}

variable "memory" {
  type        = number
  description = "Memória da task"
}

variable "desired_count" {
  type        = number
  description = "Quantidade de tasks"
}
