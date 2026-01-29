provider "aws" {
  region = var.region
}

resource "aws_ecs_cluster" "this" {
  name = "${local.name_prefix}-ecs-cluster"

  tags = local.tags
}

resource "aws_ecs_cluster_capacity_providers" "this" {
  cluster_name = aws_ecs_cluster.this.name

  capacity_providers = local.is_prod
    ? ["FARGATE"]
    : ["FARGATE_SPOT"]

  default_capacity_provider_strategy {
    capacity_provider = local.is_prod ? "FARGATE" : "FARGATE_SPOT"
    weight            = 1
  }
}

resource "aws_security_group" "ecs" {
  name   = "${local.name_prefix}-sg"
  vpc_id = data.aws_vpc.default.id

  ingress {
    from_port   = var.container_port
    to_port     = var.container_port
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = local.tags
}

resource "aws_ecs_service" "this" {
  depends_on = [
    aws_ecs_cluster_capacity_providers.this
  ]

  name    = "${local.name_prefix}-ecs-service"
  cluster = aws_ecs_cluster.this.id

  task_definition = aws_ecs_task_definition.this.arn
  desired_count   = var.desired_count

  capacity_provider_strategy {
    capacity_provider = local.is_prod ? "FARGATE" : "FARGATE_SPOT"
    weight            = 1
  }

  launch_type = null

  network_configuration {
    subnets         = data.aws_subnets.default.ids
    security_groups = [aws_security_group.ecs.id]
    assign_public_ip = true
  }

  lifecycle {
    ignore_changes = [desired_count]
  }

  tags = local.tags
}
